namespace Cygnus.Interfaces
{
    public interface IConnectionService
    {
        void AddObserver(IConnectionObserver connectionObserver);

        Task DiscoverGauges();
        void CancelDiscover();

        Task ConnectToGauge(IConnectionInformation gauge);
        IGauge? ConnectedGauge { get; }
    }
}