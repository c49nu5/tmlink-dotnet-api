using Cygnus.BLE.Interfaces;

namespace Cygnus.BLE.API.Interfaces;

public interface IBLEGaugeInternal : IBLEGauge
{
    internal string DeviceIdentifier { get; }

    internal Task<bool> Connect();
    internal void SetDevice(IBLEDevice device);
}
