using Cygnus.Interfaces;

namespace Cygnus.TMLink.API.Interfaces;

public interface ITMLinkConnectionService : IConnectionService
{
    /// <summary>
    /// Localization property for the message displayed when no Bluetooth is available on the device.
    /// </summary>
    string NoBluetoothMessage { set; }
    /// <summary>
    /// Localization property for the message displayed when the device is checking for BLE devices for TM-Link services.
    /// </summary>
    string CheckingGaugeMessageFormat { set; }
    /// <summary>
    /// Localization property for the message displayed when no TM-Link gauges are found during scanning.
    /// </summary>
    string NoTMLinkGaugesMessage { set; }
    /// <summary>
    /// Localization property for the message displayed when an error occurs during scanning for TM-Link gauges.
    /// </summary>
    string ScanningErrorMessageFormat { set; }
    /// <summary>
    /// Localization property for the message displayed when scanning for TM-Link gauges.
    /// </summary>
    string ScanningMessage { set; }
    /// <summary>
    /// Localization property for the message displayed when an error occurs during connecting to TM-Link gauges.
    /// </summary>
    string ErrorConnectingMessageFormat { set; }

    internal void GaugeIsDisconnected(string deviceIdentifier);
}
