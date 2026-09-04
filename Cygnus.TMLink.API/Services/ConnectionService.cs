using Cygnus.Interfaces;
using Cygnus.Models;
using Cygnus.TMLink.API.Interfaces;
using Cygnus.TMLink.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cygnus.TMLink.API.Services;

internal class ConnectionService : ObservableModel<IConnectionObserver>, ITMLinkConnectionService
{
    private readonly ILogger<ITMLinkConnectionService> _logger;
    private readonly IPlatformService _platformService;
    private readonly ITMLinkDeviceDiscoverer _deviceDiscoverer;
    private readonly Func<ITMLinkGauge> _gaugeFactory;
    private IGauge? _connectedGauge;

    #region Constructor
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
    #endregion

    #region Properties
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

    public string ScanningMessage { private get; set; } = "Scanning for TM-Link gauges.";
    public string NoBluetoothMessage { private get; set; } = "For TM-Link gauges, enable bluetooth and give the app the required permissions.";
    public string CheckingGaugeMessageFormat { private get; set; } = "Checking gauge {0}...";
    public string NoTMLinkGaugesMessage { private get; set; } = "No TM-Link gauges found.";
    public string ScanningErrorMessageFormat { private get; set; } = "An error occurred while scanning for TM-Link gauges. ({0})";
    public string ErrorConnectingMessageFormat { private get; set; } = "An error occurred while connecting to the gauge {0}";
    #endregion


    #region Methods
    public async Task ConnectToGauge(IConnectionInformation connectionInformation)
    {
        try
        {
            _deviceDiscoverer.Cancel();

            ConnectedGauge?.Disconnect();

            NotifyObservers(o => o.ConnectionState = ConnectionState.Connecting);
            NotifyObservers(o => o.AddConnectionMessage(string.Format(CheckingGaugeMessageFormat, connectionInformation.Name)));
            _logger.LogInformation("Connecting to device {Name}", connectionInformation.Name);

            var internalGauge = connectionInformation as ITMLinkGauge;
            if (internalGauge != null && (internalGauge.IsConnected == true || await internalGauge.Connect()))
            {
                ConnectedGauge = internalGauge;

                _logger.LogInformation("Connected to gauge {Name}", connectionInformation.Name);
            }
            else
            {
                _logger.LogInformation("Connect to gauge {Name} failed", connectionInformation.Name);
                NotifyObservers(o => o.AddConnectionMessage(string.Format(ErrorConnectingMessageFormat, connectionInformation.Name)));
                ConnectedGauge = null;
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Connection to gauge {Name} cancelled", connectionInformation.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Problem connecting to {Name}", connectionInformation.Name);
            NotifyObservers(o => o.AddConnectionMessage(string.Format(ErrorConnectingMessageFormat, connectionInformation.Name)));
            ConnectedGauge = null;
        }
    }

    public async Task DiscoverGauges()
    {
        NotifyObservers(o => o.ConnectionState = ConnectionState.Connecting);
        NotifyObservers(o => o.AddConnectionMessage(ScanningMessage));

        if (!await _platformService.CheckBluetoothConfiguration())
        {
            _logger.LogInformation("Aborting scan attempt");
            NotifyObservers(o =>
            {
                o.AddConnectionMessage(NoBluetoothMessage);
                o.ConnectionState = ConnectionState.Errored;
            });
        }
        else
        {
            try
            {
                bool gaugeDiscovered = false;
                var discoveredDevices = await _deviceDiscoverer.FindDevices();
                foreach (var device in discoveredDevices)
                {
                    var gauge = _gaugeFactory();
                    gauge.SetDevice(device);
                    _logger.LogInformation("Found device: {Name} ({DeviceIdentifier})", gauge.Name, gauge.DeviceIdentifier);
                    NotifyObservers(o =>
                    {
                        o.AddConnectionMessage(string.Format(CheckingGaugeMessageFormat, gauge.Name));
                    });

                    if (await gauge.Connect() && gauge.SerialNumber != 0)
                    {
                        gaugeDiscovered = true;
                        await gauge.Disconnect();
                        await Task.Delay(300);
                        NotifyObservers(o => o.GaugeDiscovered(gauge));
                    }
                }

                if (!gaugeDiscovered)
                {
                    NotifyObservers(o =>
                    {
                        o.AddConnectionMessage(NoTMLinkGaugesMessage);
                        o.ConnectionState = ConnectionState.Errored;
                    });
                }
                else
                {
                    NotifyObservers(o => o.ConnectionState = ConnectionState.Disconnected);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Problem discovering devices");
                NotifyObservers(o =>
                {
                    o.AddConnectionMessage(string.Format(ScanningErrorMessageFormat, ex.Message));
                    o.ConnectionState = ConnectionState.Errored;
                });
            }
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
    #endregion
}