namespace Cygnus.BLE.Interfaces
{
    public interface IBLEDevice : IDisposable
    {
        string Id { get; }
        string Name { get; }
        bool IsConnected { get; }

        Task Connect();
        Task<IBLECharacteristic[]?> GetCharacteristics(string serviceId);
        Task RequestMtuAsync(int mtu);
        void AddObserver(IBLEDeviceMonitor observer);
    }
}
