using Cygnus.Models;
using Cygnus.TMLink.Interfaces;
using InTheHand.Bluetooth;

namespace Cygnus.BLE.InTheHand
{
    internal class BLEDevice : ObservableModel<ITMLinkDeviceMonitor>, ITMLinkDevice
    {
        private BluetoothDevice _device;
        private bool disposedValue;
        private bool initialized = false;

        public BLEDevice(BluetoothDevice device)
        {
            _device = device;
        }

        public string Id => _device.Id;

        public string Name => _device.Name;

        public bool IsConnected => _device.Gatt.IsConnected;

        public async Task Connect()
        {
            await _device.Gatt.ConnectAsync();
            if (_device.Gatt.IsConnected && !initialized)
            {
                await _device.Gatt.RequestMtuAsync(500);
                _device.GattServerDisconnected += OnDisconnected;
                initialized = true;
            }
        }

        public async Task<ITMLinkCharacteristic[]?> GetCharacteristics(string serviceId)
        {
            var service = await _device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new Guid(serviceId)));
            var characteristics = await service.GetCharacteristicsAsync();
            return characteristics?.Select(c => new BLECharacteristic(c)).ToArray();
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
                    if (initialized)
                    {
                        _device.GattServerDisconnected -= OnDisconnected;
                    }

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