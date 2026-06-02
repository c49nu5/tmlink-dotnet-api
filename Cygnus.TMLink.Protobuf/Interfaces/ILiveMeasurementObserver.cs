using Cygnus.Models;

namespace Cygnus.TMLink.Interfaces
{
    public interface ILiveMeasurementObserver
    {
        void OnLiveMeasurementReceived(LiveMeasurement liveMeasurement);
    }
}