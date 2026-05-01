namespace Cygnus.Models
{
    public class GaugeRecordSummary {
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public uint FileSize { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public RecordType RecordType { get; set; }
        public uint NumberOfPointsRequired { get; set; }
        public uint NumberOfPointsTaken { get; set; }
    }
}
