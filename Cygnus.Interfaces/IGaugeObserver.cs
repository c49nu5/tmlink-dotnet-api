using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface IGaugeObserver
    {
        void OnLiveMeasurementReceived(LiveMeasurement liveMeasurement);
    }
}