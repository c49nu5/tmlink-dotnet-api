namespace Cygnus.Models
{
    public class LiveMeasurement
    {
        public uint Thickness { get; set; }

        public uint Velocity { get; set; }

        public MeasurementUnits Units { get; set; }

        public UTMode Mode { get; set; }

        public uint BatteryLevel { get; set; }

        public uint GaindB { get; set; }

        public uint Index { get; set; }

        public uint SurfaceTemp { get; set; }

        public bool IsDeepcoat { get; set; }

        public bool IsFrozen { get; set; }

        public bool IsStable { get; set; }

        public bool IsValid { get; set; }

        public AScan? AScan { get; set; }
    }
}
