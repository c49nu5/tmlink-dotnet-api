using Cygnus.Models;

namespace Cygnus.BLE.Interfaces
{
    public interface ILiveMeasurementObserver
    {
        void UpdateLiveMeasurement(LiveMeasurement liveMeasurement);
    }
}