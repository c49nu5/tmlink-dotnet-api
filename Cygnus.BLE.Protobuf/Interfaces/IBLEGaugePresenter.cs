using Cygnus.Models;

namespace Cygnus.BLE.Protobuf.Interfaces
{
    public interface IBLEGaugePresenter
    {
        string Name { get; set; }
        string SerialNumber { get; set; }

        void UpdateLiveMeasurement(LiveMeasurement liveMeasurement);
    }
}