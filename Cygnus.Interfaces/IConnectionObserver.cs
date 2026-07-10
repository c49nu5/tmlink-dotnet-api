using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface IConnectionObserver
    {
        ConnectionState ConnectionState { get; set; }
        void AddConnectionMessage(string message);
        void GaugeDiscovered(IConnectionInformation gauge);
        void GaugeConnected(IGauge? gauge);
    }
}