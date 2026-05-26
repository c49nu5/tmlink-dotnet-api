using Cygnus.BLE.API.Interfaces;
using Cygnus.BLE.API.Models;
using Cygnus.BLE.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cygnus.BLE.API.Services;

internal class ConnectionService : ObservableModel<IConnectionMonitor>, IConnectionService
{
    private readonly ILogger<IConnectionService> _logger;
    private readonly IPlatformService _platformService;
    private readonly IGaugeDiscoverer _gaugeDiscoverer;
    private readonly Func<IBLEGaugeInternal> _gaugeFactory;
    private IBLEGauge? _connectedGauge;

    public ConnectionService(
        ILogger<IConnectionService> logger,
        IPlatformService platformService,
        IGaugeDiscoverer gaugeDiscoverer,
        Func<IBLEGaugeInternal> gaugeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _platformService = platformService ?? throw new ArgumentNullException(nameof(platformService));
        _gaugeDiscoverer = gaugeDiscoverer ?? throw new ArgumentNullException(nameof(gaugeDiscoverer));
        _gaugeFactory = gaugeFactory ?? throw new ArgumentNullException(nameof(gaugeFactory));
    }

    public IBLEGauge? ConnectedGauge
    {
        get => _connectedGauge;
        set
        {
            _connectedGauge = value;
            NotifyObservers(o => o.GaugeConnected(value));
        }
    }

    public async Task ConnectToGauge(IBLEGauge gauge)
    {
        _logger.LogInformation("Connecting to device {Name}", gauge.Name);

        try
        {
            CancelDiscover();

            ConnectedGauge?.Disconnect();

            var bleGauge = gauge as IBLEGaugeInternal;
            if (bleGauge != null && (bleGauge.IsConnected == true || await bleGauge.Connect()))
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
                var discoveredDevices = await _gaugeDiscoverer.FindDevices();
                foreach (var device in discoveredDevices)
                {
                    var bleGauge = _gaugeFactory();
                    bleGauge.SetDevice(device);
                    _logger.LogInformation("Found device: {Name} ({DeviceIdentifier})", bleGauge.Name, bleGauge.DeviceIdentifier);
                    if (await bleGauge.Connect() && !string.IsNullOrWhiteSpace(bleGauge.SerialNumber))
                    {
                        NotifyObservers(o => o.GaugeDiscovered(bleGauge));
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
        _gaugeDiscoverer.Cancel();
        NotifyObservers(o => o.IsScanning = false);
    }

    public void GaugeIsDisconnected(string deviceIdentifier)
    {
        var connectedGauge = ConnectedGauge as IBLEGaugeInternal;
        if (connectedGauge != null && connectedGauge.DeviceIdentifier == deviceIdentifier)
        {
            _logger.LogInformation("Device {Name} disconnected", connectedGauge.Name);
            ConnectedGauge = null;
        }
    }
}


