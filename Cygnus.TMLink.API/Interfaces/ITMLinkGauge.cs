using Cygnus.Interfaces;
using Cygnus.TMLink.Interfaces;

namespace Cygnus.TMLink.API.Interfaces;

public interface ITMLinkGauge : IGauge
{
    internal string DeviceIdentifier { get; }

    internal Task<bool> Connect();
    internal void SetDevice(ITMLinkDevice device);
}
