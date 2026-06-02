using Cygnus.Models;
using Cygnus.TMLink.Interfaces;

namespace Cygnus.TMLink.API.Interfaces;

public interface ITMLinkGaugeInternal : ITMLinkGauge
{
    internal string DeviceIdentifier { get; }

    internal Task<bool> Connect();
    internal void SetDevice(ITMLinkDevice device);
}
