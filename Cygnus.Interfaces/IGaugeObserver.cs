using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface IGaugeObserver
    {
        void OnLiveMeasurementReceived(LiveMeasurement liveMeasurement);
        void OnPropertiesUpdated(IGauge gauge);
    }
}