using Cygnus.BLE.Interfaces;

namespace Cygnus.BLE.API.Interfaces
{
    public interface IConnectionMonitor
    {
        bool IsScanning { get; set; }
        void GaugeDiscovered(IBLEGauge gauge);
        void GaugeConnected(IBLEGauge? gauge);
    }
}