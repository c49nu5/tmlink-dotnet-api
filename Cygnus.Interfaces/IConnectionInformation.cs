using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface IConnectionInformation
    {
        ConnectionType ConnectionType { get; }
        GaugeType GaugeType { get; }
        string Port { get; }
        string Name { get; }
        uint SerialNumber { get; }
    }
}