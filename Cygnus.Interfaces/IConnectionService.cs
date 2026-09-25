namespace Cygnus.Interfaces
{
    public interface IConnectionService
    {
        void AddObserver(IConnectionObserver connectionObserver);

        Task DiscoverGauges(CancellationToken cancellationToken);

        Task ConnectToGauge(IConnectionInformation gauge);
        IGauge? ConnectedGauge { get; }
    }
}