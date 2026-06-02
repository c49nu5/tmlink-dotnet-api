using Cygnus.Models;

namespace Cygnus.TMLink.Interfaces
{
    public interface ITMLinkGaugeMonitor
    {
        void OnLiveMeasurementReceived(LiveMeasurement liveMeasurement);
    }
}