using Cygnus.BLE.API.Interfaces;
using Cygnus.BLE.Protobuf;
using InTheHand.Bluetooth;
using Microsoft.Extensions.Logging;

namespace Cygnus.BLE.API.Services;

internal class GaugeDiscoverer : IGaugeDiscoverer
{
    private readonly BluetoothUuid TMLinkServiceUuid = new Guid(Constants.TMLinkServiceId);
    private readonly ILogger<GaugeDiscoverer> _logger;
    private readonly Func<IBLEGauge> _gaugeFactory;
    private CancellationTokenSource? _scanCancellationTokenSource = null;

    public GaugeDiscoverer(ILogger<GaugeDiscoverer> logger, Func<IBLEGauge> gaugeFactory)
    {
        _logger = logger;
        _gaugeFactory = gaugeFactory;
    }

    public void Cancel()
    {
        _scanCancellationTokenSource?.Cancel();
    }

    public async Task<IEnumerable<IBLEGauge>> FindGauges()
    {
        Dictionary<string, IBLEGauge> gauges = [];
        _scanCancellationTokenSource = new(TimeSpan.FromSeconds(20));
        EventHandler<BluetoothAdvertisingEvent> onAdvertisementReceived = (object? s, BluetoothAdvertisingEvent ad) =>
        {
            _logger.LogTrace("BLE advert received from {Device} with {UuidCount} uuids containing TML Service {HasServiceId}", ad?.Device?.Id, ad?.Uuids?.Length, ad?.Uuids?.Contains(TMLinkServiceUuid));
            if (ad?.Device != null && ad.Uuids.Contains(TMLinkServiceUuid))
            {
                _logger.LogInformation("BLE TM Link device found: {DeviceId}", ad.Device.Id);
                ad.Device.Gatt.AutoConnect = true;
                gauges[ad.Device.Id] = _gaugeFactory().SetDevice(ad.Device);
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
            await Task.Delay(TimeSpan.FromSeconds(10), _scanCancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("BLE scan cancelled.");
        }
        finally
        {
            _logger.LogInformation("BLE scan complete.");
            bleScan?.Stop();
            bleScan = null;
            Bluetooth.AdvertisementReceived -= onAdvertisementReceived;
            _scanCancellationTokenSource?.Dispose();
            _scanCancellationTokenSource = null;
        }

        return gauges.Values;
    }
}