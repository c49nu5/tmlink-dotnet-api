using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface IConnectionMonitor
    {
        ConnectionState ConnectionState { get; set; }
        void ConnectionMessagesChanged(string[] message);
        void GaugeDiscovered(IGauge gauge);
        void GaugeConnected(IGauge? gauge);
    }
}