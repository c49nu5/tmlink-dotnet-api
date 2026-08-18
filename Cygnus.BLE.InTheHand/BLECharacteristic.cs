using Cygnus.TMLink.Interfaces;
using InTheHand.Bluetooth;
using Microsoft.Extensions.Logging;

namespace Cygnus.BLE.InTheHand
{
    internal class BLECharacteristic : ITMLinkCharacteristic
    {
        private readonly ILogger<BLECharacteristic> _logger;
        private readonly GattCharacteristic _characteristic;

        public BLECharacteristic(
            ILogger<BLECharacteristic> logger,
            GattCharacteristic characteristic)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _characteristic = characteristic;
            characteristic.CharacteristicValueChanged += OnCharacteristicValueChanged;
        }

        private void OnCharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
        {
            CharacteristicValueChanged?.Invoke(this, new ValueChangedEventArgs { Value = e.Value ?? [] });
        }

        public event EventHandler<ValueChangedEventArgs>? CharacteristicValueChanged;

        public string Uuid => _characteristic.Uuid.Value.ToString();

        public async Task<byte[]> ReadValue()
        {
            byte[] data = [];
            try
            {
                var value = await _characteristic.ReadValueAsync();
                if (value != null)
                {
                    data = value;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading characteristic {Id}", Uuid);
            }

            return data;
        }

        public async Task StartNotifications()
        {
            try
            {
                await _characteristic.StartNotificationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting notifications for characteristic {Id}", Uuid);
            }
        }

        public async Task StopNotifications()
        {
            try
            {
                await _characteristic.StopNotificationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping notifications for characteristic {Id}", Uuid);
            }
        }

        public async Task WriteValueWithResponse(byte[] bytes)
        {
            await _characteristic.WriteValueWithResponseAsync(bytes);
        }
    }
}