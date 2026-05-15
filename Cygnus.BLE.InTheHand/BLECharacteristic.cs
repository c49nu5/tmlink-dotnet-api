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

        public EventHandler<BLECharacteristicValueChangedEventArgs> CharacteristicValueChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string? Uuid => throw new NotImplementedException();

        public Task<byte[]?> ReadValue()
        {
            throw new NotImplementedException();
        }

        public Task StartNotifications()
        {
            throw new NotImplementedException();
        }

        public Task WriteValueWithResponse(byte[] bytes)
        {
            throw new NotImplementedException();
        }
    }
}