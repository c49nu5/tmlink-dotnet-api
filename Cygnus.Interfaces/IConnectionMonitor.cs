namespace Cygnus.Interfaces
{
    public interface IConnectionMonitor
    {
        bool IsScanning { get; set; }
        void GaugeDiscovered(IGauge gauge);
        void GaugeConnected(IGauge? gauge);
    }
}