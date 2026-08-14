using Cygnus.TMLink.Interfaces;
using InTheHand.Bluetooth;
using Microsoft.Extensions.Logging;

namespace Cygnus.BLE.InTheHand;

internal class TMLinkDeviceDiscoverer : ITMLinkDeviceDiscoverer
{
    private readonly BluetoothUuid TMLinkServiceUuid = new Guid(Constants.TMLinkServiceId);
    private readonly ILogger<TMLinkDeviceDiscoverer> _logger;
    private CancellationTokenSource? _scanCancellationTokenSource = null;

    public TMLinkDeviceDiscoverer(ILogger<TMLinkDeviceDiscoverer> logger)
    {
        _logger = logger;
    }

    public void Cancel()
    {
        _scanCancellationTokenSource?.Cancel();
    }

    public async Task<IEnumerable<ITMLinkDevice>> FindDevices()
    {
        Dictionary<string, ITMLinkDevice> gauges = [];
        _scanCancellationTokenSource = new(TimeSpan.FromSeconds(20));
        EventHandler<BluetoothAdvertisingEvent> onAdvertisementReceived = (object? s, BluetoothAdvertisingEvent ad) =>
        {
            _logger.LogTrace("BLE advert received from {Device} with {UuidCount} uuids containing TML Service {HasServiceId}", ad?.Device?.Id, ad?.Uuids?.Length, ad?.Uuids?.Contains(TMLinkServiceUuid));
            if (ad?.Device != null && ad.Uuids.Contains(TMLinkServiceUuid))
            {
                _logger.LogInformation("BLE TM Link device found: {DeviceId}", ad.Device.Id);
                 gauges[ad.Device.Id] = new BLEDevice(ad.Device);
            }
        };
        
        BluetoothLEScan? bleScan = null;
        try
        {
            BluetoothLEScanOptions options = new();
            BluetoothLEScanFilter serviceFilter = new();
            serviceFilter.Services.Add(TMLinkServiceUuid);
            options.Filters.Add(serviceFilter);
            options.AcceptAllAdvertisements = false;
            Bluetooth.AdvertisementReceived += onAdvertisementReceived;
            _logger.LogInformation("BLE scan starting.");
            bleScan = await Bluetooth.RequestLEScanAsync(options);
            _logger.LogInformation("BLE scan in progress.");
            await Task.Delay(TimeSpan.FromSeconds(5), _scanCancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("BLE scan cancelled.");
        }
        finally
        {
            Bluetooth.AdvertisementReceived -= onAdvertisementReceived;
            _logger.LogInformation("BLE scan complete.");
            bleScan?.Stop();
            bleScan = null;
            _scanCancellationTokenSource?.Dispose();
            _scanCancellationTokenSource = null;
        }

        return gauges.Values;
    }
}