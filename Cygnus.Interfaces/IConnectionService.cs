namespace Cygnus.Interfaces
{
    public interface IConnectionService
    {
        void AddObserver(IConnectionMonitor connectionMonitor);

        Task DiscoverGauges();
        void CancelDiscover();

        Task ConnectToGauge(IGauge gauge);
        IGauge? ConnectedGauge { get; }
    }
}