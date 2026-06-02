namespace Cygnus.TMLink.Interfaces
{
    public interface ITMLinkDevice : IDisposable
    {
        string Id { get; }
        string Name { get; }
        bool IsConnected { get; }

        Task Connect();
        Task<ITMLinkCharacteristic[]?> GetCharacteristics(string serviceId);
        void AddObserver(ITMLinkDeviceMonitor observer);
    }
}
