using Cygnus.BLE.API.Interfaces;
using Cygnus.BLE.API.Services;
using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Cygnus.Interfaces;
using Cygnus.Models;
using Microsoft.Extensions.Logging;

namespace Cygnus.BLE.API.Models
{
    internal class BLEGauge : ObservableModel<IGaugeMonitor>, IBLEGaugeInternal, IBLEDeviceMonitor
    {
        private readonly ILogger _logger;
        private readonly Func<byte, IProtobufChannel?> _protobufChannelFactory;
        private readonly IConnectionService _connectionService;
        private IProtobufChannel _protobufChannel = new ProtobufNullChannel();
        private IBLEDevice? _device;
        private bool _isDisposed;

        public BLEGauge(ILogger<BLEGauge> logger,
                        Func<byte, IProtobufChannel?> protobufChannelFactory,
                        IConnectionService connectionService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _protobufChannelFactory = protobufChannelFactory ?? throw new ArgumentNullException(nameof(protobufChannelFactory));
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        }

        public void SetDevice(IBLEDevice device)
        {
            _device = device;
            DeviceIdentifier = device.Id;
            Name = device.Name;
            _device.AddObserver(this);
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

                if (!_device.IsConnected)
                {
                    await _device.Connect();
                }

                if (_device.IsConnected)
                {
                    if (!_protobufChannel.IsInitialized)
                    {
                        await InitializeProtobufChannel();
                    }

                    if (_protobufChannel.IsInitialized)
                    {
                        IsConnected = true;

                        var gaugeInformation = await _protobufChannel.Connect(_device);
                        if (gaugeInformation != null)
                        {
                            SerialNumber = gaugeInformation.SerialNumber.ToString();
                        }
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

        public async Task<GaugeRecord?> GetRecord(ITransferRequest transferRequest, bool withAScans)
        {
            return await _protobufChannel.GetRecord(transferRequest, withAScans);
        }

        public async Task DeleteAllRecords()
        {
            await _protobufChannel.DeleteAllRecords();
        }

        public async Task DeleteRecord(IDeleteRequest deleteRequest)
        {
            await _protobufChannel.DeleteRecord(deleteRequest);
        }

        public async Task NewRecord(BlankRecord record)
        {
            await _protobufChannel.NewRecord(record);
        }

        public async Task<List<GaugeRecordSummary>?> GetRecordList()
        {
            return await _protobufChannel.GetRecordList();
        }

        public async Task SubscribeToLiveUpdates()
        {
            await _protobufChannel.SubscribeToLiveUpdates();
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
            _connectionService.GaugeIsDisconnected(DeviceIdentifier);
        }

        public void UpdateLiveMeasurement(LiveMeasurement liveMeasurement)
        {
            NotifyObservers(o => o.UpdateLiveMeasurement(liveMeasurement));
        }

        public void DeviceDisconnected(string deviceId)
        {
            _logger.LogInformation("Device {DeviceIdentifier} disconnected", deviceId);
            Disconnect();
        }

        private async Task InitializeProtobufChannel()
        {
            if (_device == null)
            {
                _logger.LogWarning("Cannot get protobuf version for gauge {DeviceIdentifier} because device is null", DeviceIdentifier);
                return;
            }

            byte protobufVersion = 0;
            try
            {
                var genericCharacteristics = await _device.GetCharacteristics(Constants.GenericAccessServiceId);
                if (genericCharacteristics != null)
                {
                    genericCharacteristics.TryGetValue(Constants.DeviceNameCharacteristicId, out var deviceNameCharacteristic);
                    Name = deviceNameCharacteristic != null
                        ? System.Text.Encoding.UTF8.GetString((await deviceNameCharacteristic.ReadValue()) ?? [])
                        : _device.Name;
                    _logger.LogInformation("Device name {Name}", Name);
                }

                var characteristics = await _device.GetCharacteristics(Constants.DeviceInformationServiceId);
                if (characteristics == null)
                {
                    _logger.LogWarning("Cannot get protobuf version for gauge {DeviceIdentifier} because service is null", DeviceIdentifier);
                    return;
                }
                else
                {
                    _logger.LogInformation("Checking device information service characteristics {Count} {Id1}", characteristics.Count, characteristics.Keys.FirstOrDefault());
                    characteristics.TryGetValue(Constants.DeviceModelCharacteristicId, out var deviceModelCharacteristic);
                    Model = deviceModelCharacteristic != null
                        ? System.Text.Encoding.UTF8.GetString((await deviceModelCharacteristic.ReadValue()) ?? [])
                        : string.Empty;
                    _logger.LogInformation("Device model {Model}", Model);

                    characteristics.TryGetValue(Constants.SerialNumberCharacteristicId, out var serialNumberCharacteristic);
                    SerialNumber = serialNumberCharacteristic != null
                        ? System.Text.Encoding.UTF8.GetString((await serialNumberCharacteristic.ReadValue()) ?? [])
                        : string.Empty;
                    _logger.LogInformation("Device serial number {SerialNumber}", SerialNumber);

                    characteristics.TryGetValue(Constants.FirmwareRevisionCharacteristicId, out var firmwareCharacteristic);
                    FirmwareVersion = firmwareCharacteristic != null
                        ? Version.TryParse(System.Text.Encoding.UTF8.GetString((await firmwareCharacteristic.ReadValue()) ?? []), out var version) ? version : null
                        : null;
                    _logger.LogInformation("Device firmware version {FirmwareVersion}", FirmwareVersion);

                    _logger.LogInformation("Getting protobuf version for gauge {DeviceIdentifier}", DeviceIdentifier);
                    characteristics.TryGetValue(Constants.SoftwareVersionCharacteristicId, out var characteristic);
                    if (characteristic != null)
                    {
                        var value = await characteristic.ReadValue();
                        if (value?.Length > 0)
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
                protobufChannel.AddObserver(this);
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
                        _protobufChannel.Dispose();
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