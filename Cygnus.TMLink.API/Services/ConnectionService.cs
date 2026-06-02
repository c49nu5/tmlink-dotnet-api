using Cygnus.TMLink.API.Models;
using Cygnus.TMLink.API.Interfaces;
using Cygnus.TMLink.Interfaces;
using Microsoft.Extensions.Logging;
using Cygnus.Models;

namespace Cygnus.TMLink.API.Services;

internal class ConnectionService : ObservableModel<IConnectionMonitor>, IConnectionService
{
    private readonly ILogger<IConnectionService> _logger;
    private readonly IPlatformService _platformService;
    private readonly ITMLinkDeviceDiscoverer _deviceDiscoverer;
    private readonly Func<ITMLinkGaugeInternal> _gaugeFactory;
    private ITMLinkGauge? _connectedGauge;

    public ConnectionService(
        ILogger<IConnectionService> logger,
        IPlatformService platformService,
        ITMLinkDeviceDiscoverer deviceDiscoverer,
        Func<ITMLinkGaugeInternal> gaugeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _platformService = platformService ?? throw new ArgumentNullException(nameof(platformService));
        _deviceDiscoverer = deviceDiscoverer ?? throw new ArgumentNullException(nameof(deviceDiscoverer));
        _gaugeFactory = gaugeFactory ?? throw new ArgumentNullException(nameof(gaugeFactory));
    }

    public ITMLinkGauge? ConnectedGauge
    {
        get => _connectedGauge;
        set
        {
            _connectedGauge = value;
            NotifyObservers(o => o.GaugeConnected(value));
        }
    }

    public async Task ConnectToGauge(ITMLinkGauge gauge)
    {
        _logger.LogInformation("Connecting to device {Name}", gauge.Name);

        try
        {
            CancelDiscover();

            ConnectedGauge?.Disconnect();

            var internalGauge = gauge as ITMLinkGaugeInternal;
            if (internalGauge != null && (internalGauge.IsConnected == true || await internalGauge.Connect()))
            {
                ConnectedGauge = gauge;

                _logger.LogInformation("Connected to gauge {Name}", gauge.Name);
            }
            else
            {
                _logger.LogInformation("Connect to gauge {Name} failed", gauge.Name);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Connection to gauge {Name} cancelled", gauge.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Problem connecting to {Name}", gauge.Name);
            ConnectedGauge = null;
        }
    }

    public async Task DiscoverGauges()
    {
        NotifyObservers(o => o.IsScanning = true);

        if (!await _platformService.CheckBluetoothConfiguration())
        {
            _logger.LogInformation("Aborting scan attempt");
            await _platformService.ShowMessage("Please enable bluetooth and give the app the required permissions");
        }
        else
        {
            try
            {
                var discoveredDevices = await _deviceDiscoverer.FindDevices();
                foreach (var device in discoveredDevices)
                {
                    var gauge = _gaugeFactory();
                    gauge.SetDevice(device);
                    _logger.LogInformation("Found device: {Name} ({DeviceIdentifier})", gauge.Name, gauge.DeviceIdentifier);
                    if (await gauge.Connect() && !string.IsNullOrWhiteSpace(gauge.SerialNumber))
                    {
                        NotifyObservers(o => o.GaugeDiscovered(gauge));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem discovering devices");
                await _platformService.ShowMessage($"An error occurred while scanning for devices. Please try again. ({ex.Message})");
            }
        }

        NotifyObservers(o => o.IsScanning = false);
    }

    public void CancelDiscover()
    {
        _deviceDiscoverer.Cancel();
        NotifyObservers(o => o.IsScanning = false);
    }

    public void GaugeIsDisconnected(string deviceIdentifier)
    {
        var connectedGauge = ConnectedGauge as ITMLinkGaugeInternal;
        if (connectedGauge != null && connectedGauge.DeviceIdentifier == deviceIdentifier)
        {
            _logger.LogInformation("Device {Name} disconnected", connectedGauge.Name);
            ConnectedGauge = null;
        }
    }
}


