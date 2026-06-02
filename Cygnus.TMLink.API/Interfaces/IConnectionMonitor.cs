using Cygnus.TMLink.Interfaces;

namespace Cygnus.TMLink.API.Interfaces
{
    public interface IConnectionMonitor
    {
        bool IsScanning { get; set; }
        void GaugeDiscovered(ITMLinkGauge gauge);
        void GaugeConnected(ITMLinkGauge? gauge);
    }
}