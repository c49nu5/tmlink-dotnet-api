namespace Cygnus.Models
{
    public record struct GaugeInformation(
        uint SerialNumber,
        uint GaugeId,
        uint BatteryLevel,
        string PortName,
        GaugeType GaugeType,
        int StatusMessageCount,
        ProbeType ProbeType,
        GaugeFeatures SupportedFeatures,
        DateTime? GaugeTime,
        GaugeVariant? GaugeVariant,
        uint SoftwareVersionNumber)
    {
    }
}