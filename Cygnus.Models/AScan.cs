namespace Cygnus.Models
{
    public class AScan
    {
        public AScanRectify Rectify { get; set; }

        public uint AScanStart { get; set; }

        public uint AScanWidth { get; set; }

        public byte[] AScanPoints { get; set; } = [];

        public uint Echo1 { get; set; }

        public uint Echo2 { get; set; }

        public uint Echo3 { get; set; }

    }
}
