using Cygnus.BLE.Interfaces;
using InTheHand.Bluetooth;

namespace Cygnus.BLE.API.Services
{
    internal class BLECharacteristic : IBLECharacteristic
    {
        private GattCharacteristic c;

        public BLECharacteristic(GattCharacteristic c)
        {
            this.c = c;
        }

        public event EventHandler<BLECharacteristicValueChangedEventArgs>? CharacteristicValueChanged;

        public string? Uuid => c.Uuid.Value.ToString();

        public async Task<byte[]?> ReadValue()
        {
            return await c.ReadValueAsync();
        }

        public async Task StartNotifications()
        {
            await c.StartNotificationsAsync();
        }

        public async Task WriteValueWithResponse(byte[] bytes)
        {
            await c.WriteValueWithResponseAsync(bytes);
        }
    }
}