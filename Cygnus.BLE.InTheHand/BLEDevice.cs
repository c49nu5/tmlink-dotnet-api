using Cygnus.Models;
using Cygnus.TMLink.Interfaces;
using InTheHand.Bluetooth;

namespace Cygnus.BLE.InTheHand
{
    internal class BLEDevice : ObservableModel<ITMLinkDeviceObserver>, ITMLinkDevice
    {
        private readonly Func<GattCharacteristic, BLECharacteristic> _characteristicFactory;
        private BluetoothDevice? _device;
        private bool isDisposed;

        public BLEDevice(
            Func<GattCharacteristic, BLECharacteristic> characteristicFactory, 
            BluetoothDevice device)
        {
            _characteristicFactory = characteristicFactory ?? throw new ArgumentNullException(nameof(characteristicFactory));
            Id = device.Id;
            _device = device;
            _device.GattServerDisconnected += OnDisconnected;
        }

        public string Id { get; }

        public string Name => _device?.Name ?? string.Empty;

        public bool IsConnected => _device?.Gatt.IsConnected == true; 

        public async Task Connect()
        {
            var device = _device;
            if (device == null)
            {
                device = _device = await BluetoothDevice.FromIdAsync(Id);
                if (device == null)
                {
                    return;
                }

                device.GattServerDisconnected += OnDisconnected;
            }

            await device.Gatt.ConnectAsync();

            if (device.Gatt.IsConnected)
            {
                await device.Gatt.RequestMtuAsync(500);
            }
        }

        public void Disconnect()
        {
            var device = _device;
            if (device != null && device.Gatt.IsConnected)
            {
                device.Gatt.Disconnect();
            }
            else
            {
                DisposeDevice(device);
                NotifyObservers(o => o.DeviceDisconnected(Id));
            }
        }

        public async Task<ITMLinkCharacteristic[]?> GetCharacteristics(string serviceId)
        {
            var device = _device;
            if (device == null)
            {
                return null;
            }

            var service = await device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new Guid(serviceId)));
            if (service == null)
            {
                return null;
            }
            
            var characteristics = await service.GetCharacteristicsAsync();
            return characteristics?.Select(_characteristicFactory).ToArray();
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            DisposeDevice(sender as BluetoothDevice);
            NotifyObservers(o => o.DeviceDisconnected(Id));
        }

        private void DisposeDevice(BluetoothDevice? device)
        {
            _device = null;
            if (device != null)
            {
                device.GattServerDisconnected -= OnDisconnected;
                device.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    DisposeDevice(_device);
                }

                isDisposed = true;
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