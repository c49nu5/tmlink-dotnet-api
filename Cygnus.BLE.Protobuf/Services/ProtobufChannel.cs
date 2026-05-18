using Cygnus.BLE.API.Services;
using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.Interfaces;
using Cygnus.Models;
using Microsoft.Extensions.Logging;

namespace Cygnus.BLE.Protobuf.Services
{
    internal abstract class ProtobufChannel<NotifyReady> : ObservableModel<ILiveMeasurementObserver>,  IProtobufChannel
        where NotifyReady : INotifyMessage, new()
    {
        private bool _isDisposed;

        private TaskCompletionSource<NotifyReady>? _requestCompletionSource;
        private IBLECharacteristic? _commandNotifyCharacteristic;
        private IBLECharacteristic? _liveMeasurementNotifyCharacteristic;

        protected ILogger _logger;
        protected IBLEDevice? _device;
        protected CancellationTokenSource? _recordTransferCts;
        protected IProtobufMessageConverter _protobufMessageConverter;

        public ProtobufChannel(ILogger logger, IProtobufMessageConverter protobufMessageConverter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _protobufMessageConverter = protobufMessageConverter ?? throw new ArgumentNullException(nameof(protobufMessageConverter));
        }

        public bool IsInitialized => true;

        public virtual async Task<GaugeInformation?> Connect(IBLEDevice device)
        {
            _device = device;

            await _device.RequestMtuAsync(500);

            var characteristics = await _device.GetCharacteristics(Constants.TMLinkServiceId);
            if (characteristics != null)
            {
                _logger.LogInformation("Checking TM-Link service");
                if (characteristics.TryGetValue(Constants.TMLinkMessageReadyCharacteristicId, out var characteristic))
                {
                    _commandNotifyCharacteristic = characteristic;
                    _commandNotifyCharacteristic.CharacteristicValueChanged += OnNotificationReceived;
                    await _commandNotifyCharacteristic.StartNotifications();
                }
                else
                {
                    _logger.LogError("Could not find notify characteristic for {Device}", _device.Name);
                }

                return await GetGaugeInformation();
            }
            else
            {
                _logger.LogError("Could not find TM Link service for {Device}", _device.Name);
            }

            return null;
        }

        public async Task SubscribeToLiveUpdates()
        {
            if (_device != null)
            {
                var characteristics = await _device.GetCharacteristics(Constants.TMLinkServiceId);
                if (characteristics != null)
                {
                    _logger.LogInformation("Checking TM Link service");
                    if (characteristics.TryGetValue(Constants.TMLinkLiveCharacteristicId, out var characteristic))
                    {
                        _liveMeasurementNotifyCharacteristic = characteristic;
                        _liveMeasurementNotifyCharacteristic.CharacteristicValueChanged += OnLiveMeasurementReceived;
                        await _liveMeasurementNotifyCharacteristic.StartNotifications();
                    }
                    else
                    {
                        _logger.LogError("Could not find notify characteristic for {Device}", _device.Name);
                    }
                }
            }
        }

        public void UnsubscribeFromLiveUpdates()
        {
            _liveMeasurementNotifyCharacteristic?.CharacteristicValueChanged -= OnLiveMeasurementReceived;
            _liveMeasurementNotifyCharacteristic = null;
        }

        public abstract Task DeleteAllRecords();
        public abstract Task DeleteRecord(IDeleteRequest deleteRequest);
        public abstract Task NewRecord(BlankRecord record);
        public abstract Task<List<GaugeRecordSummary>?> GetRecordList();
        protected abstract void UpdateLiveMeasurement(byte[] value);

        public virtual Task CancelRecordTransfer()
        {
            _recordTransferCts?.Cancel();
            _requestCompletionSource?.TrySetCanceled();
            _requestCompletionSource = null;
            return Task.CompletedTask;
        }

        public async Task<GaugeRecord?> GetRecord(ITransferRequest transferRequest, bool withAScans)
        {
            await CancelRecordTransfer();
            var recordTransferCts =  _recordTransferCts = new(TimeSpan.FromMinutes(45)); // Allow time for 5000 measurements with A-Scans to transfer
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
                    if (!recordTransferCts.Token.IsCancellationRequested)
                    {
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
                }

                if (!recordTransferCts.Token.IsCancellationRequested && transferRequest.Status != TransferStatus.Failed)
                {
                    transferRequest.Status = TransferStatus.Completed;
                    return gaugeRecord;
                }
                else
                {
                    _logger.LogInformation("Record transfer cancelled for record {RecordName}", transferRequest.Name);
                    transferRequest.Status = TransferStatus.Failed;
                }
            }
            else
            {
                _logger.LogInformation("No record retrieved for record {RecordName}", transferRequest.Name);
                transferRequest.Status = TransferStatus.Failed;
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

            if (_commandNotifyCharacteristic != null)
            {
                _commandNotifyCharacteristic.CharacteristicValueChanged -= OnNotificationReceived;
                _commandNotifyCharacteristic = null;
                _recordTransferCts?.Cancel();
                _requestCompletionSource?.TrySetCanceled();
                _requestCompletionSource = null;
            }
        }

        protected async Task<T?> SendCommandWithResponse<T, M>(ICommand gaugeCommand, Func<M, T> responseHandler) 
            where T : class
            where M : IMessage
        {
            if (_device == null)
            {
                _logger.LogWarning("Cannot get information for gauge because device is null");
                return null;
            }

            try
            {
                var characteristics = await _device.GetCharacteristics(Constants.TMLinkServiceId);
                if (characteristics != null)
                {
                    _logger.LogInformation("Checking TM Link service");
                    if (characteristics.TryGetValue(Constants.TMLinkWriteCommandCharacteristicId, out var commandCharacteristic))
                    {
                        _requestCompletionSource?.TrySetCanceled();
                        var requestCompletionSource = _requestCompletionSource = new TaskCompletionSource<NotifyReady>();
                        Task<NotifyReady> commandTask = requestCompletionSource.Task;

                        // Write command
                        byte[] data = _protobufMessageConverter.ToZippedProtobuf(gaugeCommand);
                        await commandCharacteristic.WriteValueWithResponse(data);

                        // Wait for notification that message is ready
                        _logger.LogInformation("Waiting for notification on characteristic {Uuid}", _commandNotifyCharacteristic?.Uuid);
                        await Task.WhenAny([commandTask, Task.Delay(TimeSpan.FromSeconds(20))]);

                        if (!requestCompletionSource.Task.IsCanceled &&
                            commandTask.IsCompleted &&
                            commandTask.Result.CommandType == gaugeCommand.CommandType &&
                            commandTask.Result.ReadDataAvailable)
                        {
                            // Read the message
                            var value = await ReadData(characteristics, new(Constants.TMLinkReadMessageCharacteristicId));
                            if (value.Length > 0)
                            {
                                var message = _protobufMessageConverter.FromZippedProtoBuf<M>(value);
                                _logger.LogInformation("Received message from gauge {Device}: {Command}", _device.Name, message.CommandType);
                                if (message.CommandType == gaugeCommand.CommandType)
                                {
                                    return responseHandler(message);
                                }
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Notification did not arrive {Command} {Completion}", gaugeCommand.CommandType, requestCompletionSource.Task.IsCompleted);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Could not find command characteristic for device {_device.Name}");
                    }
                }
            }
            catch (TaskCanceledException tex)
            {
                _logger.LogInformation(tex, "Handling command {Command} cancelled", gaugeCommand.CommandType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem handling command {Command}", gaugeCommand.CommandType);
            }

            return null;
        }

        protected async Task SendCommand(ICommand gaugeCommand, bool ignoreErrors = false)
        {
            if (_device == null)
            {
                _logger.LogWarning("Cannot send command to gauge because device is null");
                return;
            }

            var characteristics = await _device.GetCharacteristics(Constants.TMLinkServiceId);
            if (characteristics != null)
            {
                if (characteristics.TryGetValue(Constants.TMLinkWriteCommandCharacteristicId, out var commandCharacteristic))
                {
                    _logger.LogWarning("Sending command {Command} to gauge {Device}", gaugeCommand.CommandType, _device.Name);
                    _requestCompletionSource?.TrySetCanceled();
                    var requestCompletionSource = _requestCompletionSource = new TaskCompletionSource<NotifyReady>();
                    Task<NotifyReady> commandTask = requestCompletionSource.Task;

                    // Write command
                    await commandCharacteristic.WriteValueWithResponse(_protobufMessageConverter.ToZippedProtobuf(gaugeCommand));

                    // Wait for notification that command was sent
                    _logger.LogInformation("Waiting for notification on characteristic {Uuid}", _commandNotifyCharacteristic?.Uuid);
                    await Task.WhenAny([commandTask, Task.Delay(TimeSpan.FromSeconds(20))]);

                    if (!ignoreErrors &&
                        !requestCompletionSource.Task.IsCanceled &&
                        (!commandTask.IsCompleted ||
                        commandTask.Result.CommandType != gaugeCommand.CommandType ||
                        commandTask.Result.ErrorCode != ErrorCodes.Success))
                    {
                        throw new InvalidDataException($"Expected notification did not arrive {gaugeCommand.CommandType}");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Could not find command characteristic for device {_device.Name}");
                }
            }
            else
            {
                throw new InvalidOperationException($"Could not find TM Link service for device {_device.Name}");
            }    
        }

        protected async Task<T?> GetResponse<T, M>(string readCharacteristicId, Func<M, T> getGaugeInfo)
            where T : class
            where M : class
        {
            if (_device == null)
            {
                _logger.LogWarning("Cannot get information for gauge because device is null");
                return null;
            }

            try
            {
                var characteristics = await _device.GetCharacteristics(Constants.TMLinkServiceId);
                if (characteristics != null)
                {
                    _logger.LogInformation("Checking TM Link service");

                    // Read the message
                    var value = await ReadData(characteristics, readCharacteristicId);
                    if (value.Length > 0)
                    {
                        var message = _protobufMessageConverter.FromZippedProtoBuf<M>(value);
                        _logger.LogInformation("Received message from gauge {Device}: {MessageType}", _device.Name, message.GetType());
                        return getGaugeInfo(message);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Could not find TM Link service for device {_device.Name}");
                }
            }
            catch (TaskCanceledException tex)
            {
                _logger.LogInformation(tex, "Reading data from {Characteristic} cancelled", readCharacteristicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem reading data from {Characteristic}", readCharacteristicId);
            }

            return null;
        }

        private void OnNotificationReceived(object? sender, BLECharacteristicValueChangedEventArgs e)
        {
            _logger.LogInformation("Notification characteristic received {Time}", DateTime.Now);
            if (e.Value != null)
            {
                try
                {
                    var notifyReady = _protobufMessageConverter.FromProtobuf<NotifyReady>(e.Value);
                    _logger.LogInformation("Notification characteristic received command {Command}", notifyReady.CommandType);
                    _requestCompletionSource?.TrySetResult(notifyReady);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Problem with notification characteristic");
                }
            }

            _requestCompletionSource?.TrySetResult(new NotifyReady());
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

        private async Task<byte[]> ReadData(IDictionary<string, IBLECharacteristic> characteristics, string characteristicId)
        {
            if (characteristics.TryGetValue(characteristicId, out var characteristic))
            {
                return await ReadData(characteristic);
            }

            throw new InvalidOperationException($"Could not find characteristic {characteristicId} for device {_device?.Name}");
        }

        private async Task<byte[]> ReadData(IBLECharacteristic? characteristic)
        {
            byte[] data = [];
            try
            {
                if (characteristic != null)
                {
                    var value = await characteristic.ReadValue();
                    if (value != null)
                    {
                        data = value;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading characteristic {Id}", characteristic?.Uuid);
            }

            return data;
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