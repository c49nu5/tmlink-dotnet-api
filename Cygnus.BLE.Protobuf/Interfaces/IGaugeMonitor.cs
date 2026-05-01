using Cygnus.Models;

namespace Cygnus.BLE.Protobuf.Interfaces
{
    public interface IGaugeMonitor
    {
        void UpdateLiveMeasurement(LiveMeasurement liveMeasurement);
    }
}