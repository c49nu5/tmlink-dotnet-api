using Cygnus.TMLink.Interfaces;
using Cygnus.TMLink.Protobuf.Interfaces;
using Microsoft.Extensions.Logging;
using Constants = Cygnus.TMLink.Interfaces.Constants;

namespace Cygnus.TMLink.Protobuf.Services
{
    internal abstract class ProtobufCommandHandler<NotifyMessage> : IProtobufCommandHandler
        where NotifyMessage : INotifyMessage, new()
    {
        private TaskCompletionSource<NotifyMessage>? _requestCompletionSource;
        private ITMLinkCharacteristic? _notifyMessageCharacteristic;
        private ITMLinkCharacteristic? _writeCommandCharacteristic;
        private ITMLinkCharacteristic? _readMessageCharacteristic;

        protected ILogger _logger;
        protected IProtobufMessageConverter _protobufMessageConverter;

        public ProtobufCommandHandler(ILogger logger, IProtobufMessageConverter protobufMessageConverter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _protobufMessageConverter = protobufMessageConverter ?? throw new ArgumentNullException(nameof(protobufMessageConverter));
        }

        public async Task<bool> Connect(ITMLinkCharacteristic[] characteristics)
        {
            _logger.LogError("Connecting to TM-Link command characteristics");

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

            _notifyMessageCharacteristic?.CharacteristicValueChanged -= OnNotificationReceived;
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

            return true;
        }

        public void Disconnect()
        {
            _notifyMessageCharacteristic?.CharacteristicValueChanged -= OnNotificationReceived;
            CancelCommand();
        }

        public void CancelCommand()
        {
            _logger.LogInformation("Call made to cancel command");
            _requestCompletionSource?.TrySetCanceled();
            _requestCompletionSource = null;
        }

        public async Task<T?> SendCommandWithResponse<T, M>(ICommand gaugeCommand, Func<M, T> responseHandler, CancellationToken? token = null)
            where T : class
            where M : IMessage
        {
            try
            {
                _logger.LogInformation("Checking TM Link service");
                _requestCompletionSource?.TrySetCanceled();
                var requestCompletionSource = _requestCompletionSource = new TaskCompletionSource<NotifyMessage>();

                // Write command
                byte[] data = _protobufMessageConverter.ToZippedProtobuf(gaugeCommand);
                ITMLinkCharacteristic? writeCommandCharacteristic = _writeCommandCharacteristic;
                if (writeCommandCharacteristic != null)
                {
                    await writeCommandCharacteristic.WriteValueWithResponse(data);

                    // Wait for notification that message is ready
                    _logger.LogInformation("Waiting for notification on characteristic {Uuid}", _notifyMessageCharacteristic?.Uuid);

                    var cts = CancellationTokenSource.CreateLinkedTokenSource(token ?? CancellationToken.None);
                    cts.CancelAfter(TimeSpan.FromSeconds(20));
                    CancellationToken cancellationToken = cts.Token;
                    cancellationToken.Register(() => requestCompletionSource.TrySetCanceled(cancellationToken));

                    var result = await requestCompletionSource.Task;
                    _requestCompletionSource = null;
                    if (result.CommandType == gaugeCommand.CommandType &&
                        result.ReadDataAvailable)
                    {
                        // Read the message
                        ITMLinkCharacteristic? readMessageCharacteristic = _readMessageCharacteristic;
                        if (readMessageCharacteristic != null)
                        {
                            var value = await readMessageCharacteristic.ReadValue();
                            if (value.Length > 0)
                            {
                                var message = _protobufMessageConverter.FromZippedProtobuf<M>(value);
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
                        _logger.LogInformation("Notification did not arrive {Command} {CancelRequested} {CompletedSuccessfully}", gaugeCommand.CommandType, cancellationToken.IsCancellationRequested, requestCompletionSource.Task.IsCompletedSuccessfully);
                    }
                }
            }
            catch (Exception ex)
            {
                _requestCompletionSource = null;
                _logger.LogError(ex, "Problem handling command {Command}", gaugeCommand.CommandType);
            }

            return null;
        }

        public async Task<bool> SendCommand(ICommand gaugeCommand, bool ignoreErrors = false)
        {
            try
            {
                _logger.LogWarning("Sending command {Command} to gauge", gaugeCommand.CommandType);
                _requestCompletionSource?.TrySetCanceled();
                var requestCompletionSource = _requestCompletionSource = new TaskCompletionSource<NotifyMessage>();

                // Write command
                ITMLinkCharacteristic? writeCommandCharacteristic = _writeCommandCharacteristic;
                if (writeCommandCharacteristic != null)
                {
                    await writeCommandCharacteristic.WriteValueWithResponse(_protobufMessageConverter.ToZippedProtobuf(gaugeCommand));

                    CancellationToken cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(20)).Token;
                    cancellationToken.Register(() => requestCompletionSource.TrySetCanceled(cancellationToken));

                    // Wait for notification that command was sent
                    _logger.LogInformation("Waiting for notification on characteristic {Uuid}", _notifyMessageCharacteristic?.Uuid);
                    var result = await requestCompletionSource.Task;
                    _requestCompletionSource = null;
                    if (ignoreErrors ||
                        (result.CommandType == gaugeCommand.CommandType &&
                        result.ErrorCode == ErrorCodes.Success) ||
                        (result.CommandType == CommandType.CancelRecordTransfer &&
                        result.ErrorCode == ErrorCodes.TransferCancelled))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _requestCompletionSource = null;
                _logger.LogWarning(ex, "Handling command {Command}", gaugeCommand.CommandType);
                return ignoreErrors;
            }

            return false;
        }

        private void OnNotificationReceived(object? sender, ValueChangedEventArgs e)
        {
            _logger.LogInformation("Notification characteristic received {Time:yyyy-MM-dd HH:mm:ss:fff}", DateTime.Now);
            try
            {
                var notifyReady = _protobufMessageConverter.FromProtobuf<NotifyMessage>(e.Value);
                _logger.LogInformation("Notification characteristic received command {Command} {ErrorCode} {ReadDataAvailable}", notifyReady.CommandType, notifyReady.ErrorCode, notifyReady.ReadDataAvailable);
                _requestCompletionSource?.TrySetResult(notifyReady);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem with notification characteristic");
            }

            _requestCompletionSource?.TrySetResult(new NotifyMessage());
        }
    }
}