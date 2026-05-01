namespace Cygnus.BLE.API.Interfaces;

internal interface IGaugeDiscoverer
{
    void Cancel();
    Task<IEnumerable<IBLEGauge>> FindGauges();
}