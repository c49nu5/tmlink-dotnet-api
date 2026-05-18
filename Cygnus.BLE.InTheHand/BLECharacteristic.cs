using Cygnus.BLE.Interfaces;
using InTheHand.Bluetooth;

namespace Cygnus.BLE.API.Services
{
    internal class BLECharacteristic : IBLECharacteristic
    {
        private GattCharacteristic _c;

        public BLECharacteristic(GattCharacteristic c)
        {
            _c = c;
            c.CharacteristicValueChanged += (s, e) => CharacteristicValueChanged?.Invoke(this, new BLECharacteristicValueChangedEventArgs{ Value = e.Value });
        }

        public event EventHandler<BLECharacteristicValueChangedEventArgs>? CharacteristicValueChanged;

        public string? Uuid => _c.Uuid.Value.ToString();

        public async Task<byte[]?> ReadValue()
        {
            return await _c.ReadValueAsync();
        }

        public async Task StartNotifications()
        {
            await _c.StartNotificationsAsync();
        }

        public async Task WriteValueWithResponse(byte[] bytes)
        {
            await _c.WriteValueWithResponseAsync(bytes);
        }
    }
}