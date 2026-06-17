namespace Cygnus.Models
{
    public record GaugeRecord
    {
        public uint RecordID;

        public string Name = string.Empty;

        public string Location = string.Empty;

        public string Surveyor = string.Empty;

        public string Key = string.Empty;

        public DateTime? Created;

        public DateTime? Updated;

        public RecordType RecordType;

        public IList<GaugeMeasurement> Measurements = [];

        public uint NumberPointsRequired;

        public uint NumberOfPointsTaken;

        public ProtectionState ProtectionState = ProtectionState.Open;
    }

    public record GaugeLinearRecord : GaugeRecord
    {
        public MeasurementUnits Units;

        public uint MinimumThickness;

        public uint ReferenceThickness;

        public string Notes = string.Empty;

        public RecordCreator Creator = RecordCreator.Gauge;

        public string[] Comments = [];
    }

    public record GaugeGridRecord : GaugeLinearRecord
    {
        public GridType GridType;

        public int RowCount;

        public string RowNamePrefix = string.Empty;

        public int ColumnCount;

        public string ColumnNamePrefix = string.Empty;

        public IEnumerable<string> ColumnNameList = [];
    }

    public record GaugeMultiPointRecord : GaugeGridRecord
    {
        public int PointCount;

        public string PointNamePrefix = string.Empty;
    }

    public record GaugeBScan : GaugeLinearRecord
    {
        public double ThicknessRange;

        public int MinThicknessTime;

        public double ScanLength;

        public int ScanDuration;

        public int ScanInterval;

        public ScanDirection Direction;
    }
}
