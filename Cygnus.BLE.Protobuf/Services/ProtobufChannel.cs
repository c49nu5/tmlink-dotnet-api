using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.Models;
using InTheHand.Bluetooth;
using Microsoft.Extensions.Logging;

namespace Cygnus.BLE.Protobuf.Services
{
    internal abstract class ProtobufChannel<NotifyReady> : IProtobufChannel
        where NotifyReady : INotifyMessage, new()
    {
        private bool _isDisposed;

        private TaskCompletionSource<NotifyReady>? _requestCompletionSource;
        private GattCharacteristic? _commandNotifyCharacteristic;
        private GattCharacteristic? _liveMeasurementNotifyCharacteristic;

        protected ILogger _logger;
        protected BluetoothDevice? _device;
        protected CancellationTokenSource? _recordTransferCts;
        protected IBLEGaugePresenter? _gaugePresenter;

        public ProtobufChannel(ILogger logger)
        {
            _logger = logger;
        }

        public bool IsInitialized => true;

        public virtual async Task Connect(BluetoothDevice device, IBLEGaugePresenter gaugePresenter)
        {
            _device = device;

            _gaugePresenter = gaugePresenter;

            await _device.Gatt.RequestMtuAsync(500);

            var service = await _device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new(Constants.TMLinkServiceId)));
            if (service != null)
            {
                _logger.LogInformation("Checking service {Uuid}", service.Uuid);
                var characteristics = await service.GetCharacteristicsAsync();
                _commandNotifyCharacteristic = characteristics.FirstOrDefault(c => c.Uuid == Guid.Parse(Constants.TMLinkMessageReadyCharacteristicId));
                if (_commandNotifyCharacteristic != null)
                {
                    _commandNotifyCharacteristic.CharacteristicValueChanged += OnNotificationReceived;
                    await _commandNotifyCharacteristic.StartNotificationsAsync();
                }
                else
                {
                    _logger.LogError("Could not find notify characteristic for {Device}", gaugePresenter.Name);
                }

                await UpdateGaugeInformation();
            }
            else
            {
                _logger.LogError("Could not find TM Link service for {Device}", gaugePresenter.Name);
            }
        }

        public async Task SubscribeToLiveUpdates()
        {
            if (_device != null)
            {
                var service = await _device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new(Constants.TMLinkServiceId)));
                if (service != null)
                {
                    _logger.LogInformation("Checking service {Uuid}", service.Uuid);
                    var characteristics = await service.GetCharacteristicsAsync();
                    _liveMeasurementNotifyCharacteristic = characteristics.FirstOrDefault(c => c.Uuid == Guid.Parse(Constants.TMLinkLiveCharacteristicId));
                    if (_liveMeasurementNotifyCharacteristic != null)
                    {
                        _liveMeasurementNotifyCharacteristic.CharacteristicValueChanged += OnLiveMeasurementReceived;
                        await _liveMeasurementNotifyCharacteristic.StartNotificationsAsync();
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
            _recordTransferCts?.Cancel();
            _recordTransferCts = new(TimeSpan.FromMinutes(45)); // Allow time for 5000 measurements with A-Scans to transfer
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
                    if (!_recordTransferCts.Token.IsCancellationRequested)
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

                if (!_recordTransferCts.Token.IsCancellationRequested && transferRequest.Status != TransferStatus.Failed)
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

        protected abstract Task UpdateGaugeInformation();

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
                var service = await _device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new(Constants.TMLinkServiceId)));
                if (service != null)
                {
                    _logger.LogInformation("Checking service {Name}", service.Uuid);
                    var characteristics = await service.GetCharacteristicsAsync();
                    var commandCharacteristic = characteristics.FirstOrDefault(c => c.Uuid == Guid.Parse(Constants.TMLinkWriteCommandCharacteristicId));
                    if (commandCharacteristic != null)
                    {
                        _requestCompletionSource?.TrySetCanceled();
                        var requestCompletionSource = _requestCompletionSource = new TaskCompletionSource<NotifyReady>();
                        Task<NotifyReady> commandTask = requestCompletionSource.Task;

                        // Write command
                        byte[] data = gaugeCommand.ToZippedProtobuf();
                        await commandCharacteristic.WriteValueWithResponseAsync(data);

                        // Wait for notification that message is ready
                        _logger.LogInformation("Waiting for notification on characteristic {Uuid}", _commandNotifyCharacteristic?.Uuid);
                        await Task.WhenAny([commandTask, Task.Delay(TimeSpan.FromSeconds(20))]);

                        if (commandTask.IsCompleted &&
                            commandTask.Result.CommandType == gaugeCommand.CommandType &&
                            commandTask.Result.ReadDataAvailable)
                        {
                            // Read the message
                            var value = await ReadData(characteristics, new(Constants.TMLinkReadMessageCharacteristicId));
                            if (value.Length > 0)
                            {
                                var message = value.FromZippedProtoBuf<M>();
                                _logger.LogInformation("Received message from gauge {DeviceIdentifier}: {Command}", _device.Id, message.CommandType);
                                if (message.CommandType == gaugeCommand.CommandType)
                                {
                                    return responseHandler(message);
                                }
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Notification did not arrive {Command} {Completion}", gaugeCommand.CommandType, _requestCompletionSource.Task.IsCompleted);
                        }

                        _requestCompletionSource = null;
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

            var service = await _device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new(Constants.TMLinkServiceId)));
            if (service != null)
            {
                var characteristics = await service.GetCharacteristicsAsync();
                var commandCharacteristic = characteristics.FirstOrDefault(c => c.Uuid == Guid.Parse(Constants.TMLinkWriteCommandCharacteristicId));
                if (commandCharacteristic != null)
                {
                    _logger.LogWarning("Sending command {Command} to gauge {DeviceIdentifier}", gaugeCommand.CommandType, _device.Id);
                    _requestCompletionSource?.TrySetCanceled();
                    var requestCompletionSource = _requestCompletionSource = new TaskCompletionSource<NotifyReady>();
                    Task<NotifyReady> commandTask = requestCompletionSource.Task;

                    // Write command
                    await commandCharacteristic.WriteValueWithResponseAsync(gaugeCommand.ToZippedProtobuf());

                    // Wait for notification that command was sent
                    _logger.LogInformation("Waiting for notification on characteristic {Uuid}", _commandNotifyCharacteristic?.Uuid);
                    await Task.WhenAny([commandTask, Task.Delay(TimeSpan.FromSeconds(20))]);

                    if (!ignoreErrors && 
                        (!commandTask.IsCompleted ||
                        commandTask.Result.CommandType != gaugeCommand.CommandType ||
                        commandTask.Result.ErrorCode != ErrorCodes.Success))
                    {
                        throw new InvalidDataException($"Expected notification did not arrive {gaugeCommand.CommandType}");
                    }
                }
            }
        }

        protected async Task<T?> GetResponse<T, M>(Guid readCharacteristicId, Func<M, T> getGaugeInfo)
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
                var service = await _device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new(Constants.TMLinkServiceId)));
                if (service != null)
                {
                    _logger.LogInformation("Checking service {Name}", service.Uuid);
                    var characteristics = await service.GetCharacteristicsAsync();

                    // Read the message
                    var value = await ReadData(characteristics, readCharacteristicId);
                    if (value.Length > 0)
                    {
                        var message = value.FromZippedProtoBuf<M>();
                        _logger.LogInformation("Received message from gauge {DeviceIdentifier}: {MessageType}", _device.Id, message.GetType());
                        return getGaugeInfo(message);
                    }
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

        private void OnNotificationReceived(object? sender, GattCharacteristicValueChangedEventArgs e)
        {
            _logger.LogInformation("Notification characteristic received {Time}", DateTime.Now);
            if (e.Value != null)
            {
                try
                {
                    var notifyReady = e.Value.FromProtobuf<NotifyReady>();
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

        private void OnLiveMeasurementReceived(object? sender, GattCharacteristicValueChangedEventArgs e)
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

        private async Task<byte[]> ReadData(IReadOnlyList<GattCharacteristic> characteristics, Guid characteristicId)
        {
            var characteristic = characteristics.FirstOrDefault(c => c.Uuid == characteristicId);
            return await ReadData(characteristic);
        }

        private async Task<byte[]> ReadData(GattCharacteristic? characteristic)
        {
            byte[] data = [];
            try
            {
                if (characteristic?.Properties.HasFlag(GattCharacteristicProperties.Read) == true)
                {
                    using var queryCts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
                    var value = await characteristic.ReadValueAsync();
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