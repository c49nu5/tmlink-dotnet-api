namespace Cygnus.BLE.API.Interfaces;

public interface IConnectionService
{
    void AddObserver(IConnectionMonitor connectionMonitor);

    Task DiscoverGauges();
    void CancelDiscover();

    Task ConnectToGauge(IBLEGauge gauge);
    IBLEGauge? ConnectedGauge { get; }

    internal void GaugeIsConnectedChanged(string deviceIdentifier);
}
