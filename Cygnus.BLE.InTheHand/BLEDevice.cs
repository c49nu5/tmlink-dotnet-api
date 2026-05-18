using Cygnus.BLE.Interfaces;
using InTheHand.Bluetooth;

namespace Cygnus.BLE.API.Services
{
    internal class BLEDevice : ObservableModel<IBLEDeviceMonitor>, IBLEDevice
    {
        private BluetoothDevice _device;
        private bool disposedValue;

        public BLEDevice(BluetoothDevice device)
        {
            _device = device;
            _device.GattServerDisconnected += OnDisconnected;
        }

        public string Id => _device.Id;

        public string Name => _device.Name;

        public bool IsConnected => _device.Gatt.IsConnected;

        public Task Connect()
        {
            return _device.Gatt.ConnectAsync();
        }

        public async Task<IDictionary<string, IBLECharacteristic>?> GetCharacteristics(string serviceId)
        {
            var service = await _device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new Guid(serviceId)));
            var characteristics = await service.GetCharacteristicsAsync();
            return characteristics?.ToDictionary(c => c.Uuid.Value.ToString(), c => (IBLECharacteristic)new BLECharacteristic(c), StringComparer.InvariantCultureIgnoreCase);
        }

        public Task RequestMtuAsync(int mtu)
        {
            return _device.Gatt.RequestMtuAsync(mtu);
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            NotifyObservers(o => o.DeviceDisconnected(Id));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _device.GattServerDisconnected -= OnDisconnected;
                    _device.Dispose();
                }

                disposedValue = true;
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