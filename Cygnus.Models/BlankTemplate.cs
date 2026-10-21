namespace Cygnus.Models
{
    public class BlankTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public uint Velocity { get; set; }
        public uint NumberOfPoints { get; set; }
        public MeasurementUnits Units { get; set; }
        public BlankPoint[] MeasurementPoints { get; set; } = [];
        public UserField[] UserFields { get; set; } = [];
    }
}
