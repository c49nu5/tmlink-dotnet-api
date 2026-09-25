namespace Cygnus.TMLink.Interfaces;

public interface ITMLinkDeviceDiscoverer
{
    Task<IEnumerable<ITMLinkDevice>> FindDevices(CancellationToken cancellationToken);
}
