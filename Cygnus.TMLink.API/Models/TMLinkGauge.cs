using Cygnus.TMLink.Interfaces;
using Cygnus.TMLink.Protobuf.Interfaces;
using Cygnus.TMLink.Protobuf.Services;
using Cygnus.Interfaces;
using Cygnus.Models;
using Microsoft.Extensions.Logging;
using Cygnus.TMLink.API.Interfaces;

namespace Cygnus.TMLink.API.Models
{
    internal class TMLinkGauge : ObservableModel<IGaugeObserver>, ITMLinkGauge, ITMLinkDeviceObserver, ILiveMeasurementObserver
    {
        private readonly ILogger _logger;
        private readonly Func<byte, IProtobufChannel?> _protobufChannelFactory;
        private readonly ITMLinkConnectionService _connectionService;
        private IProtobufChannel _protobufChannel = new ProtobufNullChannel();
        private ITMLinkDevice? _device;
        private bool _isDataTransferInProgress;
        private bool _isDisposed;

        public TMLinkGauge(ILogger<TMLinkGauge> logger,
                        Func<byte, IProtobufChannel?> protobufChannelFactory,
                        ITMLinkConnectionService connectionService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _protobufChannelFactory = protobufChannelFactory ?? throw new ArgumentNullException(nameof(protobufChannelFactory));
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));
        }

        public void SetDevice(ITMLinkDevice device)
        {
            _device = device;
            DeviceIdentifier = device.Id;
            Name = device.Name;
            _device.AddObserver(this);
        }

        public ConnectionType ConnectionType => ConnectionType.TMLink;

        public string DeviceIdentifier { get; private set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public uint SerialNumber { get; set; } = 0;
        public Version? FirmwareVersion { get; set; }
        public bool IsConnected { get; set; }
        public GaugeType GaugeType => GaugeType.M5EX; // TODO : Only one gauge type for now, but may need to be dynamic if we support more in the future
        public string Port => "BLE";
        public uint SoftwareVersionNumber { get; set; } = 0;
        public GaugeVariant? GaugeVariant { get; set; }
        public uint GaugeId { get; set; } = 0;
        public uint BatteryLevel { get; set; } = 0;
        public uint StatusMessageCount { get; set; }

        public bool IsDataTransferInProgress
        {
            get => _isDataTransferInProgress;
            set
            {
                if (_isDataTransferInProgress != value)
                {
                    _isDataTransferInProgress = value;
                    NotifyObservers(o => o.OnPropertiesUpdated(this));
                }
            }
        }

        public int MaxMaterialCount => 100; // From the M5EX manual, the gauge supports up to 100 materials
        public int MinCommentCount => 8;

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
                        var gaugeInformation = await _protobufChannel.Connect(_device);
                        if (gaugeInformation != null)
                        {
                            IsConnected = true;
                            SerialNumber = gaugeInformation?.SerialNumber ?? 0;
                            SoftwareVersionNumber = gaugeInformation?.SoftwareVersionNumber ?? 0;
                            GaugeVariant = gaugeInformation?.GaugeVariant;
                            BatteryLevel = gaugeInformation?.BatteryLevel ?? 0;
                            GaugeId = gaugeInformation?.GaugeId ?? 0;
                        }
                        else
                        {
                            _logger.LogError("Failed to retrieve gauge information for {Name}", Name);
                            _protobufChannel.Dispose();
                            _protobufChannel = new ProtobufNullChannel();
                        }
                    }
                }
                else
                {
                    _logger.LogError("Called Connect but {Device} still not connected", Name);
                    return false;
                }

                return _protobufChannel.IsInitialized;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to gauge {DeviceIdentifier}", DeviceIdentifier);
                return false;
            }
        }

        public async Task<GaugeRecord?> GetRecord(IFileTransferRequest transferRequest, bool withAScans)
        {
            return await _protobufChannel.GetRecord(new TMLinkTransferMonitor(i => IsDataTransferInProgress = i, transferRequest), withAScans);
        }

        public async Task DeleteAllRecords()
        {
            await _protobufChannel.DeleteAllRecords();
        }

        public async Task DeleteRecord(IFileTransferRequest deleteRequest)
        {
            await _protobufChannel.DeleteRecord(new TMLinkTransferMonitor(i => IsDataTransferInProgress = i, deleteRequest));
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

        public void OnLiveMeasurementReceived(LiveMeasurement liveMeasurement)
        {
            if (!liveMeasurement.IsFrozen)
            {
                BatteryLevel = liveMeasurement.BatteryLevel;
                StatusMessageCount = liveMeasurement.PointIndex;
                NotifyObservers(o => o.OnPropertiesUpdated(this));
            }

            NotifyObservers(o => o.OnLiveMeasurementReceived(liveMeasurement));
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
                    var deviceNameCharacteristic = genericCharacteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.DeviceNameCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
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
                    _logger.LogInformation("Checking device information service characteristics {Count} {Id1}", characteristics.Count(), characteristics.FirstOrDefault()?.Uuid);
                    var deviceModelCharacteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.DeviceModelCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
                    Model = deviceModelCharacteristic != null
                        ? System.Text.Encoding.UTF8.GetString((await deviceModelCharacteristic.ReadValue()) ?? [])
                        : string.Empty;
                    _logger.LogInformation("Device model {Model}", Model);

                    var serialNumberCharacteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.SerialNumberCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
                    SerialNumber = serialNumberCharacteristic != null
                        ? uint.TryParse(System.Text.Encoding.UTF8.GetString((await serialNumberCharacteristic.ReadValue()) ?? []), out var serialNumber) ? serialNumber : 0
                        : 0;
                    _logger.LogInformation("Device serial number {SerialNumber}", SerialNumber);

                    var firmwareCharacteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.FirmwareRevisionCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
                    FirmwareVersion = firmwareCharacteristic != null
                        ? Version.TryParse(System.Text.Encoding.UTF8.GetString((await firmwareCharacteristic.ReadValue()) ?? []), out var version) ? version : null
                        : null;
                    _logger.LogInformation("Device firmware version {FirmwareVersion}", FirmwareVersion);

                    _logger.LogInformation("Getting protobuf version for gauge {DeviceIdentifier}", DeviceIdentifier);
                    var characteristic = characteristics.FirstOrDefault(c => c.Uuid.Equals(Constants.SoftwareVersionCharacteristicId, StringComparison.InvariantCultureIgnoreCase));
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

        public GaugeFeatures SupportedFeatures =>
            GaugeFeatures.CanCancelRecordTransfer |
            GaugeFeatures.CanDeleteBScans |
            GaugeFeatures.CanDeleteRecords |
            GaugeFeatures.HasAScanCapability |
            GaugeFeatures.HasBScanCapability |
            GaugeFeatures.HasDeepCoat |
            GaugeFeatures.SendsAScans |
            GaugeFeatures.SendsBatteryLevel |
            GaugeFeatures.SendsLiveMeasurements |
            GaugeFeatures.CanSendBScanList |
            GaugeFeatures.CanSendRecordList;

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

        #region Methods only implemented in CygLink gauges at present, but not in TMLink gauges. These methods are here to satisfy the IGauge interface.
        public ProbeType ProbeType => ProbeType.None;

        public DateTime? GaugeTime => throw new NotSupportedException();

        public ErrorCode DoProbeZero() => throw new NotSupportedException();

        public void SendCommentList(string[] commentsList) => throw new NotSupportedException();

        public void SendMaterialList(List<Material> materialList) => throw new NotSupportedException();

        public ErrorCode SendVelocity(uint velocity, MeasurementUnits units) => throw new NotSupportedException();

        public ErrorCode SendMeasurementSetup(IMeasurementSettingsUpdate measurementSettingsUpdate, MeasurementUnits units, MeasurementResolution resolution) => throw new NotSupportedException();

        public ErrorCode SendMeasurementSetup(MeasurementUnits units, MeasurementResolution resolution) => throw new NotSupportedException();

        public ErrorCode SetGaugeTime(DateTime gaugeTime) 
        {
            // When the gauge is connected it sends a get gauge information command with the current time,
            // a side-effect of which is that the gauge time is automatically updated, so we don't need to set the time on the gauge.
            return ErrorCode.Success; 
        }
        #endregion
    }
}