using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface IConnectionObserver
    {
        ConnectionState ConnectionState { get; set; }
        void ConnectionMessagesChanged(string[] message);
        void GaugeDiscovered(IConnectionInformation gauge);
        void GaugeConnected(IGauge? gauge);
    }
}