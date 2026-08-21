using Cygnus.Interfaces;
using Cygnus.Models;
using Cygnus.TMLink.Interfaces;
using Cygnus.TMLink.Protobuf.Interfaces;
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
        private ILiveMeasurementObserver? _gauge;
        protected CancellationTokenSource? _recordTransferCts;
        protected IProtobufCommandHandler _protobufCommandHandler;

        public ProtobufChannel(ILogger logger, IProtobufCommandHandler protobufCommandHandler)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _protobufCommandHandler = protobufCommandHandler ?? throw new ArgumentNullException(nameof(protobufCommandHandler));
        }

        public bool IsInitialized => true;

        public async Task<bool> Connect(ITMLinkDevice device, ILiveMeasurementObserver gauge)
        {
            _device = device;
            _gauge = gauge;

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

                return true;
            }

            return false;
        }

        protected override void OnIsBeingObservedChanged(bool isBeingObserved)
        {
            var liveCharacteristic = _liveCharacteristic;
            if (liveCharacteristic != null)
            {
                if (isBeingObserved)
                {
                    liveCharacteristic.CharacteristicValueChanged += OnLiveMeasurementReceived;
                    liveCharacteristic.StartNotifications();
                }
                else
                {
                    liveCharacteristic.CharacteristicValueChanged -= OnLiveMeasurementReceived;
                    // await liveCharacteristic.StopNotifications(); // TODO This works with the virtual device but not with the real device. Need to investigate why.
                }
            }
        }

        public abstract Task DeleteAllRecords();
        public abstract Task DeleteRecord(IFileTransferRequest deleteRequest);
        public abstract Task NewRecord(BlankRecord record);
        public abstract Task<List<GaugeRecordSummary>?> GetRecordList();

        protected abstract void ProcessLiveMeasurement(byte[] value);

        public virtual Task<bool> CancelRecordTransfer()
        {
            bool transferInProgress = false;
            CancellationTokenSource? recordTransferCts = _recordTransferCts;
            if (recordTransferCts != null && !recordTransferCts.IsCancellationRequested)
            {
                _logger.LogInformation("Cancelling record transfer");
                recordTransferCts.Cancel();
                transferInProgress = true;
            }
            
            _recordTransferCts = null;
            return Task.FromResult(transferInProgress);
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
                            GaugeMeasurement? measurement = transferRequest.RecordType == RecordType.BScan
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
                            if (gaugeRecord is GaugeBScan bScanRecord && bScanRecord.Measurements.Count > 0)
                            {
                                var maxWidth = bScanRecord.Measurements.Max(m => m.AScan.WidthThickness);
                                bScanRecord.ThicknessRange = maxWidth;
                            }

                            transferRequest.Status = FileTransferState.Complete;
                            return gaugeRecord;
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No record retrieved for record {RecordName}", transferRequest.Name);
                        transferRequest.Status = FileTransferState.Error;
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Record transfer cancelled for record {RecordName}", transferRequest.Name);
                    transferRequest.Status = FileTransferState.Error;
                }
            }

            _recordTransferCts = null;

            return null;
        }

        protected abstract Task<GaugeRecord?> GetGaugeRecord(IFileTransferRequest transferRequest);

        protected abstract Task<GaugeMeasurement?> GetMeasurementPoint(string name, bool withAScans);

        protected abstract Task<GaugeRecord?> GetGaugeBScan(IFileTransferRequest transferRequest);

        protected abstract Task<GaugeMeasurement?> GetBScanPoint(string name, bool withAScans);

        public abstract Task<GaugeInformation?> GetGaugeInformation();

        public async Task Disconnect()
        {
            RemoveAllObservers();
            await CancelRecordTransfer();
            _protobufCommandHandler.Disconnect();
        }

        private void OnLiveMeasurementReceived(object? sender, ValueChangedEventArgs e)
        {
            _logger.LogInformation("Live measurement characteristic received {Time}", DateTime.Now);
            if (e.Value != null)
            {
                try
                {
                    ProcessLiveMeasurement(e.Value);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Problem with live measurement characteristic");
                }
            }
        }

        protected void OnLiveMeasurementReceived(LiveMeasurement liveMeasurement)
        {
            // Initially notify the gauge of the live measurement, then notify any other observers
            _gauge?.OnLiveMeasurementReceived(liveMeasurement);
            
            NotifyObservers(o => o.OnLiveMeasurementReceived(liveMeasurement));
        }

        protected uint GetThicknessTime(uint thickness, uint velocity, MeasurementUnits measurementUnits)
        {
            if (velocity == 0)
            {
                return 0;
            }

            var thicknessTime = thickness / GetNsToThicknessDivisor(velocity, measurementUnits);
            return Convert.ToUInt32(thicknessTime);
        }

        private static double GetNsToThicknessDivisor(uint velocity, MeasurementUnits measurementUnits)
        {
            return velocity / (measurementUnits == MeasurementUnits.Imperial ? 2e4 : 2e3);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Disconnect().ConfigureAwait(false);
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