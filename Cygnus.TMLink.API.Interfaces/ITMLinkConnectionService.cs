using Cygnus.Interfaces;

namespace Cygnus.TMLink.API.Interfaces;

public interface ITMLinkConnectionService : IConnectionService
{
    internal void GaugeIsDisconnected(string deviceIdentifier);
}
