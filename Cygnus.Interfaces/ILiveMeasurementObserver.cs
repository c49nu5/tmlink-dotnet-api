using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface ILiveMeasurementObserver
    {
        void OnLiveMeasurementReceived(LiveMeasurement liveMeasurement);
    }
}