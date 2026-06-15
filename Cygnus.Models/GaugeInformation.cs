namespace Cygnus.Models
{
    public record struct GaugeInformation(
        string PortName,
        GaugeType GaugeType,
        uint GaugeId,
        uint SerialNumber,
        int StatusMessageCount,
        ProbeType ProbeType,
        GaugeFeatures SupportedFeatures,
        uint BatteryLevel,
        DateTime? GaugeTime,
        GaugeVariant? GaugeVariant = null,
        uint SoftwareVersionNumber = 0)
    {
    }
}