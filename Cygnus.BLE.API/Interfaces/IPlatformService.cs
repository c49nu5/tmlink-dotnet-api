namespace Cygnus.BLE.API.Interfaces;

public interface IPlatformService
{
    Task<bool> CheckBluetoothConfiguration();
    Task ShowMessage(string message, string cancel = "");
}
