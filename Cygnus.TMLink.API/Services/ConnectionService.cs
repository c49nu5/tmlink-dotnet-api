using Cygnus.TMLink.API.Interfaces;
using Cygnus.TMLink.Interfaces;
using Microsoft.Extensions.Logging;
using Cygnus.Models;
using Cygnus.Interfaces;

namespace Cygnus.TMLink.API.Services;

internal class ConnectionService : ObservableModel<IConnectionObserver>, ITMLinkConnectionService
{
    private readonly ILogger<ITMLinkConnectionService> _logger;
    private readonly IPlatformService _platformService;
    private readonly ITMLinkDeviceDiscoverer _deviceDiscoverer;
    private readonly Func<ITMLinkGauge> _gaugeFactory;
    private IGauge? _connectedGauge;

    public ConnectionService(
        ILogger<ITMLinkConnectionService> logger,
        IPlatformService platformService,
        ITMLinkDeviceDiscoverer deviceDiscoverer,
        Func<ITMLinkGauge> gaugeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _platformService = platformService ?? throw new ArgumentNullException(nameof(platformService));
        _deviceDiscoverer = deviceDiscoverer ?? throw new ArgumentNullException(nameof(deviceDiscoverer));
        _gaugeFactory = gaugeFactory ?? throw new ArgumentNullException(nameof(gaugeFactory));
    }

    public IGauge? ConnectedGauge
    {
        get => _connectedGauge;
        set
        {
            _connectedGauge = value;
            NotifyObservers(o =>
            {
                o.GaugeConnected(value);
                o.ConnectionState = value != null ? ConnectionState.Connected : ConnectionState.Disconnected;
            });
        }
    }

    public async Task ConnectToGauge(IConnectionInformation connectionInformation)
    {
        _logger.LogInformation("Connecting to device {Name}", connectionInformation.Name);

        try
        {
            CancelDiscover();

            ConnectedGauge?.Disconnect();

            var internalGauge = connectionInformation as ITMLinkGauge;
            if (internalGauge != null && (internalGauge.IsConnected == true || await internalGauge.Connect()))
            {
                ConnectedGauge = internalGauge;

                _logger.LogInformation("Connected to gauge {Name}", connectionInformation.Name);
            }
            else
            {
                _logger.LogInformation("Connect to gauge {Name} failed", connectionInformation.Name);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Connection to gauge {Name} cancelled", connectionInformation.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Problem connecting to {Name}", connectionInformation.Name);
            ConnectedGauge = null;
        }
    }

    public async Task DiscoverGauges()
    {
        NotifyObservers(o => o.ConnectionState = ConnectionState.Connecting);

        if (!await _platformService.CheckBluetoothConfiguration())
        {
            _logger.LogInformation("Aborting scan attempt");
            NotifyObservers(o =>
            {
                o.AddConnectionMessage("Please enable bluetooth and give the app the required permissions");
                o.ConnectionState = ConnectionState.Errored;
            });
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
                    NotifyObservers(o =>
                    {
                        o.AddConnectionMessage($"Checking device {gauge.Name}...");
                    });

                    if (await gauge.Connect() && gauge.SerialNumber != 0)
                    {
                        NotifyObservers(o => o.GaugeDiscovered(gauge));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem discovering devices");
                NotifyObservers(o =>
                {
                    o.AddConnectionMessage($"An error occurred while scanning for devices. Please try again. ({ex.Message})");
                    o.ConnectionState = ConnectionState.Errored;
                });

                return;
            }

            NotifyObservers(o => o.ConnectionState = ConnectionState.Disconnected);
        }
    }

    public void CancelDiscover()
    {
        _deviceDiscoverer.Cancel();
        NotifyObservers(o => o.ConnectionState = ConnectionState.Disconnected);
    }

    public void GaugeIsDisconnected(string deviceIdentifier)
    {
        var connectedGauge = ConnectedGauge as ITMLinkGauge;
        if (connectedGauge != null && connectedGauge.DeviceIdentifier == deviceIdentifier)
        {
            _logger.LogInformation("Device {Name} disconnected", connectedGauge.Name);
            ConnectedGauge = null;
        }
    }
}


