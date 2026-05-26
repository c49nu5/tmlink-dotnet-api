namespace Cygnus.Models
{
    public class GaugeInformation
    {
        public uint SerialNumber { get; set; }
        public uint GaugeUD { get; set; }
        public uint VersionNumber { get; set; }
        public uint BatteryLevel { get; set; }
        public GaugeVariant GaugeVariant { get; set; }
    }
}
