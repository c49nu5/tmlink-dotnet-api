using Cygnus.BLE.API.Interfaces;
using Cygnus.BLE.API.Services;
using Cygnus.BLE.Protobuf;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Cygnus.Models;
using InTheHand.Bluetooth;
using Microsoft.Extensions.Logging;

namespace Cygnus.BLE.API.Models
{
    internal class BLEGauge : ObservableService<IGaugeMonitor>, IBLEGauge
    {
        private readonly ILogger _logger;
        private readonly Func<byte, IProtobufChannel?> _protobufChannelFactory;
        private readonly IConnectionService _connectionService;
        private IProtobufChannel _protobufChannel = new ProtobufNullChannel();
        private BluetoothDevice? _device;
        private bool _isDisposed;

        public BLEGauge(ILogger<BLEGauge> logger,
                        Func<byte, IProtobufChannel?> protobufChannelFactory,
                        IConnectionService connectionService)
        {
            _logger = logger;
            _protobufChannelFactory = protobufChannelFactory;
            _connectionService = connectionService;
        }

        public IBLEGauge SetDevice(BluetoothDevice device)
        {
            _device = device;
            DeviceIdentifier = device.Id;
            Name = device.Name;
            _device.GattServerDisconnected += OnGattServerDisconnected;
            return this;
        }

        public string DeviceIdentifier { get; private set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public Version? FirmwareVersion { get; set; }
        public bool IsConnected { get; set; }

        public async Task<bool> Connect()
        {
            try
            {
                if (_device == null)
                {
                    _logger.LogError("No device to connect to for gauge {DeviceIdentifier}", DeviceIdentifier);
                    return false;
                }

                if (!_device.Gatt.IsConnected)
                {
                    await _device.Gatt.ConnectAsync();
                }

                if (_device.Gatt.IsConnected)
                {
                    IsConnected = true;

                    _connectionService.GaugeIsConnectedChanged(DeviceIdentifier);

                    if (!_protobufChannel.IsInitialized)
                    {
                        await InitializeProtobufChannel();
                    }

                    if (_protobufChannel.IsInitialized)
                    {
                        await _protobufChannel.Connect(_device, this);
                    }
                }
                else
                {
                    _logger.LogError("Called Connect but {Device} still not connected", Name);
                }

                return _protobufChannel.IsInitialized;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error connecting to gauge {DeviceIdentifier}", DeviceIdentifier);
                return false;
            }
        }

        public Task<GaugeRecord?> GetRecord(ITransferRequest transferRequest, bool withAScans)
        {
            return _protobufChannel.GetRecord(transferRequest, withAScans);
        }

        public Task DeleteAllRecords()
        {
            return _protobufChannel.DeleteAllRecords();
        }

        public Task DeleteRecord(IDeleteRequest deleteRequest)
        {
            return _protobufChannel.DeleteRecord(deleteRequest);
        }

        public Task NewRecord(BlankRecord record)
        {
            return _protobufChannel.NewRecord(record);
        }

        public async Task<List<GaugeRecordSummary>?> GetRecordList()
        {
            return await _protobufChannel.GetRecordList();
        }

        public Task SubscribeToLiveUpdates()
        {
            return _protobufChannel.SubscribeToLiveUpdates();
        }

        public void UnsubscribeFromLiveUpdates()
        {
            _protobufChannel.UnsubscribeFromLiveUpdates();
        }

        public async Task CancelRecordTransfer()
        {
            await _protobufChannel.CancelRecordTransfer();
        }

        public void Disconnect()
        {
            IsConnected = false;
            _protobufChannel.Disconnect();
            _connectionService.GaugeIsConnectedChanged(DeviceIdentifier);
        }

        public void UpdateLiveMeasurement(LiveMeasurement liveMeasurement)
        {
            NotifyObservers(o => o.UpdateLiveMeasurement(liveMeasurement));
        }

        private void OnGattServerDisconnected(object? sender, EventArgs e)
        {
            _logger.LogInformation("Device {DeviceIdentifier} disconnected", DeviceIdentifier);
            Disconnect();
        }

        private async Task InitializeProtobufChannel()
        {
            if (_device == null)
            {
                _logger.LogWarning("Cannot get protobuf version for gauge {DeviceIdentifier} because device is null", DeviceIdentifier);
                return;
            }

            byte protobufVersion = 1;
            try
            {
                var genericService = await _device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new(Constants.GenericAccessServiceId)));
                if (genericService != null)
                {
                    var deviceNameCharacteristic = await genericService.GetCharacteristicAsync(Guid.Parse(Constants.DeviceNameCharacteristicId));
                    Name = deviceNameCharacteristic != null
                        ? System.Text.Encoding.UTF8.GetString((await deviceNameCharacteristic.ReadValueAsync()) ?? [])
                        : _device.Name;
                }

                var deviceInformationService = await _device.Gatt.GetPrimaryServiceAsync(BluetoothUuid.FromGuid(new(Constants.DeviceInformationServiceId)));
                if (deviceInformationService == null)
                {
                    _logger.LogWarning("Cannot get protobuf version for gauge {DeviceIdentifier} because service is null", DeviceIdentifier);
                    return;
                }
                else
                {
                    _logger.LogInformation("Checking device service {Uuid}", deviceInformationService.Uuid);
                    var characteristics = await deviceInformationService.GetCharacteristicsAsync();
                    var deviceModelCharacteristic = characteristics.FirstOrDefault(c => c.Uuid == Guid.Parse(Constants.DeviceModelCharacteristicId));
                    Model = deviceModelCharacteristic != null
                        ? System.Text.Encoding.UTF8.GetString((await deviceModelCharacteristic.ReadValueAsync()) ?? [])
                        : string.Empty;

                    var serialNumberCharacteristic = characteristics.FirstOrDefault(c => c.Uuid == Guid.Parse(Constants.SerialNumberCharacteristicId));
                    SerialNumber = serialNumberCharacteristic != null
                        ? System.Text.Encoding.UTF8.GetString((await serialNumberCharacteristic.ReadValueAsync()) ?? [])
                        : string.Empty;

                    var firmwareCharacteristic = characteristics.FirstOrDefault(c => c.Uuid == Guid.Parse(Constants.FirmwareRevisionCharacteristicId));
                    FirmwareVersion = firmwareCharacteristic != null
                        ? Version.TryParse(System.Text.Encoding.UTF8.GetString((await firmwareCharacteristic.ReadValueAsync()) ?? []), out var version) ? version : null
                        : null;

                    _logger.LogInformation("Getting protobuf version for gauge {DeviceIdentifier}", DeviceIdentifier);
                    var characteristic = characteristics.FirstOrDefault(c => c.Uuid == Guid.Parse(Constants.SoftwareVersionCharacteristicId));
                    if (characteristic != null)
                    {
                        var value = await characteristic.ReadValueAsync();
                        if (value.Length > 0)
                        {
                            protobufVersion = byte.Parse(System.Text.Encoding.UTF8.GetString(value));
                            _logger.LogInformation("Received protobuf version {Version} from gauge {DeviceIdentifier}", protobufVersion, DeviceIdentifier);
                        }
                        else
                        {
                            _logger.LogInformation("Did not receive protobuf version from gauge {DeviceIdentifier}", DeviceIdentifier);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Could not retrieve protobuf characteristic for gauge {DeviceIdentifier}", DeviceIdentifier);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem loading protobuf version");
            }

            IProtobufChannel? protobufChannel = _protobufChannelFactory(protobufVersion);
            if (protobufChannel != null)
            {
                _protobufChannel = protobufChannel;
            }
            else
            {
                _logger.LogError("Protobuf version {ProtobufVersion} not supported", protobufVersion);
            }

            return;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_device != null)
                    {
                        _device.GattServerDisconnected -= OnGattServerDisconnected;
                        _device.Gatt.Disconnect();
                        _protobufChannel?.Dispose();
                        _device.Dispose();
                        _device = null;
                    }
                }

                _isDisposed = true;
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