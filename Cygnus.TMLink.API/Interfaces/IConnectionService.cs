using Cygnus.TMLink.Interfaces;

namespace Cygnus.TMLink.API.Interfaces;

public interface IConnectionService
{
    void AddObserver(IConnectionMonitor connectionMonitor);

    Task DiscoverGauges();
    void CancelDiscover();

    Task ConnectToGauge(ITMLinkGauge gauge);
    ITMLinkGauge? ConnectedGauge { get; }

    internal void GaugeIsDisconnected(string deviceIdentifier);
}
