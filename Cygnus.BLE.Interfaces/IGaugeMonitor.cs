using Cygnus.Models;

namespace Cygnus.BLE.Interfaces
{
    public interface IGaugeMonitor
    {
        void UpdateLiveMeasurement(LiveMeasurement liveMeasurement);
    }
}