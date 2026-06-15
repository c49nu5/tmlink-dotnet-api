namespace Cygnus.Models
{
    public record struct GaugeAScan
    {
        public uint StartTime;
        public uint WidthTime;
        public uint OffsetTime;
        public RectifyMode RectifyMode;
        public sbyte[] Amplitudes;
        public AScanFixedRange Range;
    }
}