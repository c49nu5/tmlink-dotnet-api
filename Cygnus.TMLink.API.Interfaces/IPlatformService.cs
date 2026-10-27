namespace Cygnus.TMLink.API.Interfaces;

public interface IPlatformService
{
    bool SupportsWiderAScans { get; }

    Task<bool> CheckBluetoothConfiguration();
}
