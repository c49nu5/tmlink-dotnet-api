using Cygnus.TMLink.Interfaces;
using InTheHand.Bluetooth;

namespace Cygnus.BLE.InTheHand
{
    internal class BLECharacteristic : ITMLinkCharacteristic
    {
        private readonly GattCharacteristic _c;

        public BLECharacteristic(GattCharacteristic c)
        {
            _c = c;
            c.CharacteristicValueChanged += OnCharacteristicValueChanged;
        }

        private void OnCharacteristicValueChanged(object sender, GattCharacteristicValueChangedEventArgs e)
        {
            CharacteristicValueChanged?.Invoke(this, new ValueChangedEventArgs { Value = e.Value ?? [] });
        }

        public event EventHandler<ValueChangedEventArgs>? CharacteristicValueChanged;

        public string Uuid => _c.Uuid.Value.ToString();

        public async Task<byte[]> ReadValue()
        {
            byte[] data = [];
            try
            {
                var value = await _c.ReadValueAsync();
                if (value != null)
                {
                    data = value;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error reading characteristic {Id} {ex}", Uuid, ex.Message);
            }

            return data;
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