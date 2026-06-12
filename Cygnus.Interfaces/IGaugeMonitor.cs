using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface IGaugeMonitor
    {
        void OnLiveMeasurementReceived(LiveMeasurement liveMeasurement);
    }
}