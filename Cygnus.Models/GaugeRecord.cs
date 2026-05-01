namespace Cygnus.Models
{
    public class GaugeRecord
    {
        public uint RecordID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Surveyor { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }

        public RecordType RecordType { get; set; }

        public IList<MeasurementPoint> Measurements { get; set; } = [];

        public uint NumberPointsRequired { get; set; }

        public uint NumberOfPointsTaken { get; set; }
    }
}
