namespace Cygnus.TMLink.API.Interfaces;

public interface IPlatformService
{
    Task<bool> CheckBluetoothConfiguration();
}
