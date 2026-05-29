using MessagePack;

namespace Cygnus.Models
{
    [MessagePackObject]
    public record struct AScan
    {
        [Key(0)]
        public uint StartTime;
        [Key(1)]
        public uint WidthTime;
        [Key(2)]
        public uint OffsetTime;
        [Key(3)]
        public RectifyMode RectifyMode;
        [Key(4)]
        public sbyte[] Amplitudes;
        [Key(5)]
        public AScanFixedRange Range;
    }
}