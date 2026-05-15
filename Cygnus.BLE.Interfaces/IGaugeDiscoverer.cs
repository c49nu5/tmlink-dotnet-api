namespace Cygnus.BLE.Interfaces;

public interface IGaugeDiscoverer
{
    void Cancel();
    Task<IEnumerable<IBLEDevice>> FindDevices();
}
