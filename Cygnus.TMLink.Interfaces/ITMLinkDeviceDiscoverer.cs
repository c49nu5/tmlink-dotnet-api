namespace Cygnus.TMLink.Interfaces;

public interface ITMLinkDeviceDiscoverer
{
    void Cancel();
    Task<IEnumerable<ITMLinkDevice>> FindDevices();
}
