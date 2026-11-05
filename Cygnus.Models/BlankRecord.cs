namespace Cygnus.Models
{
    public class BlankRecord
    {
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public RecordType Type { get; set; }
        public MeasurementUnits Units { get; set; }
        public uint ColumnCount { get; set; }
        public uint RowCount { get; set; }
        public GridType GridPattern { get; set; }
        public uint ThicknessMinLimit { get; set; }
        public uint ThicknessRefLimit { get; set; }
        public BlankPoint[] MeasurementPoints { get; set; } = [];
        public UserField[] UserFields { get; set; } = [];
    }
}
