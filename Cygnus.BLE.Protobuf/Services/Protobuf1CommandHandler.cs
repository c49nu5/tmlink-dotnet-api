using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.V1;
using Microsoft.Extensions.Logging;
using Constants = Cygnus.BLE.Interfaces.Constants;

namespace Cygnus.BLE.Protobuf.Services
{
    internal class Protobuf1CommandHandler : IProtobufCommandHandler
    {
        private TaskCompletionSource<NotifyMessage>? _requestCompletionSource;
        private IBLECharacteristic? _notifyMessageCharacteristic;
        private IBLECharacteristic? _writeCommandCharacteristic;
        private IBLECharacteristic? _readMessageCharacteristic;
        private IBLECharacteristic? _frozenCharacteristic;

        protected ILogger _logger;
        protected IProtobufMessageConverter _protobufMessageConverter;

        public Protobuf1CommandHandler(ILogger logger, IProtobufMessageConverter protobufMessageConverter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _protobufMessageConverter = protobufMessageConverter ?? throw new ArgumentNullException(nameof(protobufMessageConverter));
        }

        public async Task<bool> Connect(IEnumerable<IBLECharacteristic> characteristics)
        {
            var writeCommandCharacteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.TMLinkWriteCommandCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
            if (writeCommandCharacteristic != null)
            {
                _writeCommandCharacteristic = writeCommandCharacteristic;
            }
            else
            {
                _logger.LogError("Could not find write command characteristic for device");
                return false;
            }

            var notifyMessageCharacteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.TMLinkNotifyMessageCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
            if (notifyMessageCharacteristic != null)
            {
                _notifyMessageCharacteristic = notifyMessageCharacteristic;
                _notifyMessageCharacteristic.CharacteristicValueChanged += OnNotificationReceived;
                await _notifyMessageCharacteristic.StartNotifications();
            }
            else
            {
                _logger.LogError("Could not find notify message characteristic for device");
                return false;
            }

            var readMessageCharacteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.TMLinkReadMessageCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
            if (readMessageCharacteristic != null)
            {
                _readMessageCharacteristic = readMessageCharacteristic;
            }
            else
            {
                _logger.LogError("Could not find read message characteristic for device");
                return false;
            }

            var frozenCharacteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.TMLinkFrozenCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
            if (frozenCharacteristic != null)
            {
                _frozenCharacteristic = frozenCharacteristic;
            }
            else
            {
                _logger.LogError("Could not find frozen characteristic for device");
                return false;
            }

            return true;
        }

        public void Disconnect()
        {
            _notifyMessageCharacteristic?.CharacteristicValueChanged -= OnNotificationReceived;
            CancelCommand();
        }

        public void CancelCommand()
        {
            _requestCompletionSource?.TrySetCanceled();
            _requestCompletionSource = null;
        }

        public async Task<T?> SendCommandWithResponse<T, M>(ICommand gaugeCommand, Func<M, T> responseHandler)
            where T : class
            where M : IMessage
        {
            try
            {
                _logger.LogInformation("Checking TM Link service");
                _requestCompletionSource?.TrySetCanceled();
                var requestCompletionSource = _requestCompletionSource = new TaskCompletionSource<NotifyMessage>();
                Task<NotifyMessage> commandTask = requestCompletionSource.Task;

                // Write command
                byte[] data = _protobufMessageConverter.ToZippedProtobuf(gaugeCommand);
                IBLECharacteristic? writeCommandCharacteristic = _writeCommandCharacteristic;
                if (writeCommandCharacteristic != null)
                {
                    await writeCommandCharacteristic.WriteValueWithResponse(data);

                    // Wait for notification that message is ready
                    _logger.LogInformation("Waiting for notification on characteristic {Uuid}", _notifyMessageCharacteristic?.Uuid);
                    await Task.WhenAny([commandTask, Task.Delay(TimeSpan.FromSeconds(20))]);

                    if (!requestCompletionSource.Task.IsCanceled &&
                        commandTask.IsCompleted &&
                        commandTask.Result.CommandType == gaugeCommand.CommandType &&
                        commandTask.Result.ReadDataAvailable)
                    {
                        // Read the message
                        IBLECharacteristic? readMessageCharacteristic = _readMessageCharacteristic;
                        if (readMessageCharacteristic != null)
                        {
                            var value = await readMessageCharacteristic.ReadValue();
                            if (value.Length > 0)
                            {
                                var message = _protobufMessageConverter.FromZippedProtoBuf<M>(value);
                                _logger.LogInformation("Received message from gauge {Command}", message.CommandType);
                                if (message.CommandType == gaugeCommand.CommandType)
                                {
                                    return responseHandler(message);
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Notification did not arrive {Command} {Completion}", gaugeCommand.CommandType, requestCompletionSource.Task.IsCompleted);
                    }
                }
            }
            catch (OperationCanceledException tex)
            {
                _logger.LogInformation(tex, "Handling command {Command} cancelled", gaugeCommand.CommandType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem handling command {Command}", gaugeCommand.CommandType);
            }

            return null;
        }

        public async Task SendCommand(ICommand gaugeCommand, bool ignoreErrors = false)
        {
            try
            {
                _logger.LogWarning("Sending command {Command} to gauge", gaugeCommand.CommandType);
                _requestCompletionSource?.TrySetCanceled();
                var requestCompletionSource = _requestCompletionSource = new TaskCompletionSource<NotifyMessage>();
                Task<NotifyMessage> commandTask = requestCompletionSource.Task;

                // Write command

                IBLECharacteristic? writeCommandCharacteristic = _writeCommandCharacteristic;
                if (writeCommandCharacteristic != null)
                {
                    await writeCommandCharacteristic.WriteValueWithResponse(_protobufMessageConverter.ToZippedProtobuf(gaugeCommand));

                    // Wait for notification that command was sent
                    _logger.LogInformation("Waiting for notification on characteristic {Uuid}", _notifyMessageCharacteristic?.Uuid);
                    await Task.WhenAny([commandTask, Task.Delay(TimeSpan.FromSeconds(20))]);

                    if (!ignoreErrors &&
                        !requestCompletionSource.Task.IsCanceled &&
                        (!commandTask.IsCompleted ||
                        commandTask.Result.CommandType != gaugeCommand.CommandType ||
                        commandTask.Result.ErrorCode != Interfaces.ErrorCodes.Success))
                    {
                        throw new InvalidDataException($"Expected notification did not arrive {gaugeCommand.CommandType}");
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
        }

        public async Task<T?> GetFrozenMeasurement<T>()
            where T : class
        {
            try
            {
                _logger.LogInformation("Checking TM Link service");

                // Read the message
                var frozenCharacteristic = _frozenCharacteristic;
                if (frozenCharacteristic == null)
                {
                    _logger.LogError("Frozen characteristic not found");
                    return null;
                };

                var value = await frozenCharacteristic.ReadValue();
                if (value.Length > 0)
                {
                    var measurement = _protobufMessageConverter.FromZippedProtoBuf<T>(value);
                    _logger.LogInformation("Received message from gauge: {MessageType}", measurement.GetType());
                    return measurement;
                }
            }
            catch (TaskCanceledException tex)
            {
                _logger.LogInformation(tex, "Reading frozen measurement cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem reading frozen measurement");
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
                    var notifyReady = _protobufMessageConverter.FromProtobuf<NotifyMessage>(e.Value);
                    _logger.LogInformation("Notification characteristic received command {Command}", notifyReady.CommandType);
                    _requestCompletionSource?.TrySetResult(notifyReady);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Problem with notification characteristic");
                }
            }

            _requestCompletionSource?.TrySetResult(new NotifyMessage());
        }
    }
}