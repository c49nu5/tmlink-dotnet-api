namespace Cygnus.Models
{
    public class BlankPoint
    {
        public uint Key { get; set; }
        public string Name { get; set; } = string.Empty;
        public uint ColNumX { get; set; }
        public uint RowNumY { get; set; }
        public Method Method { get; set; }
        public uint ThicknessMinLimit { get; set; }
        public uint ThicknessMaxLimit { get; set; }
    }
}