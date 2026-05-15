using Cygnus.Models;

namespace Cygnus.BLE.Interfaces
{
    public interface IBLEGaugePresenter
    {
        string Name { get; }
        string Model { get; }
        Version? FirmwareVersion { get; }
        string SerialNumber { get; set; }

        void UpdateLiveMeasurement(LiveMeasurement liveMeasurement);
    }
}