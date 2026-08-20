namespace Cygnus.Interfaces
{
    public interface IGaugeObserver
    {
        void OnPropertiesUpdated(IGauge gauge);
    }
}