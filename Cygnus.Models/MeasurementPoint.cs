namespace Cygnus.Models
{
    public class MeasurementPoint
    {
        public uint RecordID { get; set; }

        public string Name { get; set; } = string.Empty;

        public uint Key { get; set; }

        public Method Method { get; set; }

        public MeasurementUnits Units { get; set; }

        public uint Thickness { get; set; }        //Thicnkess

        public uint Velocity { get; set; }

        public ProbeType ProbeType { get; set; }

        public UTMode Mode { get; set; }

        public DateTime? Timestamp { get; set; }

        public uint ColNumX { get; set; }

        public uint RowNumY { get; set; }

        public AScan? AScan { get; set; } 
    }
}
