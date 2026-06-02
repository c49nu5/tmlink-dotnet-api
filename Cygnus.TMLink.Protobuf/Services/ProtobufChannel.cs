using Cygnus.TMLink.Interfaces;
using Cygnus.TMLink.Protobuf.Interfaces;
using Cygnus.Interfaces;
using Cygnus.Models;
using Microsoft.Extensions.Logging;

namespace Cygnus.TMLink.Protobuf.Services
{
    internal abstract class ProtobufChannel : ObservableModel<ILiveMeasurementObserver>,  IProtobufChannel
    {
        private bool _isDisposed;

        private ITMLinkCharacteristic? _liveCharacteristic;
        protected ITMLinkCharacteristic? _frozenCharacteristic;

        protected ILogger _logger;
        protected ITMLinkDevice? _device;
        protected CancellationTokenSource? _recordTransferCts;
        protected IProtobufCommandHandler _protobufCommandHandler;

        public ProtobufChannel(ILogger logger, IProtobufCommandHandler protobufCommandHandler)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _protobufCommandHandler = protobufCommandHandler ?? throw new ArgumentNullException(nameof(protobufCommandHandler));
        }

        public bool IsInitialized => true;

        public async Task<GaugeInformation?> Connect(ITMLinkDevice device)
        {
            _device = device;

            ITMLinkCharacteristic[]? characteristics = await _device.GetCharacteristics(Constants.TMLinkServiceId);
            if (characteristics == null)
            {
                _logger.LogError("Could not find characteristics for TM Link service on device {Device}", _device.Name);
            }
            else if (await _protobufCommandHandler.Connect(characteristics))
            {
                var liveCharacteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.TMLinkLiveCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
                if (liveCharacteristic != null)
                {
                    _liveCharacteristic = liveCharacteristic;
                }
                else
                {
                    _logger.LogError("Could not find notify live characteristic for {Device}", _device.Name);
                }

                var frozenCharacteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.TMLinkFrozenCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
                if (frozenCharacteristic != null)
                {
                    _frozenCharacteristic = frozenCharacteristic;
                }
                else
                {
                    _logger.LogError("Could not find notify live characteristic for {Device}", _device.Name);
                }

                return await GetGaugeInformation();
            }

            return null;
        }

        public async Task SubscribeToLiveUpdates()
        {
            if (_liveCharacteristic != null)
            { 
                _liveCharacteristic.CharacteristicValueChanged += OnLiveMeasurementReceived;
                await _liveCharacteristic.StartNotifications();
            }
        }

        public void UnsubscribeFromLiveUpdates()
        {
            _liveCharacteristic?.CharacteristicValueChanged -= OnLiveMeasurementReceived;
        }

        public abstract Task DeleteAllRecords();
        public abstract Task DeleteRecord(IFileTransferRequest deleteRequest);
        public abstract Task NewRecord(BlankRecord record);
        public abstract Task<List<GaugeRecordSummary>?> GetRecordList();

        protected abstract void UpdateLiveMeasurement(byte[] value);

        public virtual Task CancelRecordTransfer()
        {
            _recordTransferCts?.Cancel();
            _recordTransferCts = null;
            _protobufCommandHandler.CancelCommand();
            return Task.CompletedTask;
        }

        public async Task<GaugeRecord?> GetRecord(IFileTransferRequest transferRequest, bool withAScans)
        {
            await CancelRecordTransfer();
            using (var recordTransferCts = _recordTransferCts = new(TimeSpan.FromMinutes(45))) // Allow time for 5000 measurements with A-Scans to transfer
            {
                try
                {
                    transferRequest.PercentageTransferred = 0;
                    transferRequest.Status = FileTransferState.Pending;
                    GaugeRecord? gaugeRecord = transferRequest.RecordType == RecordType.BScan
                        ? await GetGaugeBScan(transferRequest)
                        : await GetGaugeRecord(transferRequest);
                    if (gaugeRecord != null)
                    {
                        transferRequest.Status = FileTransferState.Receiving;
                        for (int i = 0; i < gaugeRecord.NumberOfPointsTaken; i++)
                        {
                            recordTransferCts.Token.ThrowIfCancellationRequested();
                            _logger.LogInformation("Transferring point {PointIndex} of {TotalPoints} for record {RecordName}", i + 1, gaugeRecord.NumberOfPointsTaken, gaugeRecord.Name);
                            Measurement? measurement = transferRequest.RecordType == RecordType.BScan
                                ? await GetBScanPoint(transferRequest.Name, withAScans)
                                : await GetMeasurementPoint(transferRequest.Name, withAScans);
                            if (measurement != null)
                            {
                                transferRequest.PercentageTransferred = (double)(i + 1) / gaugeRecord.NumberOfPointsTaken;
                                gaugeRecord.Measurements.Add(measurement);
                            }
                            else
                            {
                                _logger.LogInformation("Failed to retrieve measurement {Index} for record {RecordName}", i, transferRequest.Name);
                            }
                        }

                        _recordTransferCts = null;
                        if (transferRequest.Status != FileTransferState.Error)
                        {
                            transferRequest.Status = FileTransferState.Complete;
                            return gaugeRecord;
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No record retrieved for record {RecordName}", transferRequest.Name);
                        _recordTransferCts = null;
                        transferRequest.Status = FileTransferState.Error;
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Record transfer cancelled for record {RecordName}", transferRequest.Name);
                    transferRequest.Status = FileTransferState.Error;
                }
            }

            return null;
        }
        
        protected abstract Task<GaugeRecord?> GetGaugeRecord(IFileTransferRequest transferRequest);

        protected abstract Task<Measurement?> GetMeasurementPoint(string name, bool withAScans);

        protected abstract Task<GaugeRecord?> GetGaugeBScan(IFileTransferRequest transferRequest);

        protected abstract Task<Measurement?> GetBScanPoint(string name, bool withAScans);

        protected abstract Task<GaugeInformation> GetGaugeInformation();

        public void Disconnect()
        {
            UnsubscribeFromLiveUpdates();
            CancelRecordTransfer();
            _protobufCommandHandler.Disconnect();
        }

        private void OnLiveMeasurementReceived(object? sender, ValueChangedEventArgs e)
        {
            _logger.LogInformation("Live measurement characteristic received {Time}", DateTime.Now);
            if (e.Value != null)
            {
                try
                {
                    UpdateLiveMeasurement(e.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Problem with live measurement characteristic");
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Disconnect();
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}