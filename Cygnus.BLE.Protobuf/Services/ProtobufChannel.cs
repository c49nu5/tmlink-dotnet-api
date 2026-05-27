using Cygnus.BLE.API.Services;
using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.Interfaces;
using Cygnus.Models;
using Microsoft.Extensions.Logging;

namespace Cygnus.BLE.Protobuf.Services
{
    internal abstract class ProtobufChannel : ObservableModel<ILiveMeasurementObserver>,  IProtobufChannel
    {
        private bool _isDisposed;

        private IBLECharacteristic? _liveCharacteristic;
        protected IBLECharacteristic? _frozenCharacteristic;

        protected ILogger _logger;
        protected IBLEDevice? _device;
        protected CancellationTokenSource? _recordTransferCts;
        protected IProtobufCommandHandler _protobufCommandHandler;

        public ProtobufChannel(ILogger logger, IProtobufCommandHandler protobufCommandHandler)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _protobufCommandHandler = protobufCommandHandler ?? throw new ArgumentNullException(nameof(protobufCommandHandler));
        }

        public bool IsInitialized => true;

        public async Task<GaugeInformation?> Connect(IBLEDevice device)
        {
            _device = device;

            await _device.RequestMtuAsync(500);

            IBLECharacteristic[]? characteristics = await _device.GetCharacteristics(Constants.TMLinkServiceId);
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
        public abstract Task DeleteRecord(IDeleteRequest deleteRequest);
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

        public async Task<GaugeRecord?> GetRecord(ITransferRequest transferRequest, bool withAScans)
        {
            await CancelRecordTransfer();
            using (var recordTransferCts = _recordTransferCts = new(TimeSpan.FromMinutes(45))) // Allow time for 5000 measurements with A-Scans to transfer
            {
                try
                {
                    transferRequest.PercentageTransferred = 0;
                    transferRequest.Status = TransferStatus.Requested;
                    GaugeRecord? gaugeRecord = transferRequest.RecordType == RecordType.BScan
                        ? await GetGaugeBScan(transferRequest)
                        : await GetGaugeRecord(transferRequest);
                    if (gaugeRecord != null)
                    {
                        transferRequest.Status = TransferStatus.InProgress;
                        for (int i = 0; i < gaugeRecord.NumberOfPointsTaken; i++)
                        {
                            recordTransferCts.Token.ThrowIfCancellationRequested();
                            _logger.LogInformation("Transferring point {PointIndex} of {TotalPoints} for record {RecordName}", i + 1, gaugeRecord.NumberOfPointsTaken, gaugeRecord.Name);
                            MeasurementPoint? measurement = transferRequest.RecordType == RecordType.BScan
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
                                transferRequest.Status = TransferStatus.Failed;
                            }
                        }

                        _recordTransferCts = null;
                        if (transferRequest.Status != TransferStatus.Failed)
                        {
                            transferRequest.Status = TransferStatus.Completed;
                            return gaugeRecord;
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No record retrieved for record {RecordName}", transferRequest.Name);
                        _recordTransferCts = null;
                        transferRequest.Status = TransferStatus.Failed;
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Record transfer cancelled for record {RecordName}", transferRequest.Name);
                    transferRequest.Status = TransferStatus.Failed;
                }
            }

            return null;
        }
        
        protected abstract Task<GaugeRecord?> GetGaugeRecord(ITransferRequest transferRequest);

        protected abstract Task<MeasurementPoint?> GetMeasurementPoint(string name, bool withAScans);

        protected abstract Task<GaugeRecord?> GetGaugeBScan(ITransferRequest transferRequest);

        protected abstract Task<MeasurementPoint?> GetBScanPoint(string name, bool withAScans);

        protected abstract Task<GaugeInformation> GetGaugeInformation();

        public void Disconnect()
        {
            UnsubscribeFromLiveUpdates();
            CancelRecordTransfer();
            _protobufCommandHandler.Disconnect();
        }

        private void OnLiveMeasurementReceived(object? sender, BLECharacteristicValueChangedEventArgs e)
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