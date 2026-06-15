namespace Cygnus.Models;

public record GaugeMeasurement
{
    public Guid Id;
    public string Name = string.Empty;
    public GaugeGridCoordinate GridCoordinate;
    public uint ThicknessTime;
    public MeasurementUnits Units;
    public DateTimeOffset? Time;
    public MeasureMode Mode;
    public uint Velocity;
    public double ReferenceThickness;
    public double MinimumThickness;
    public ProbeType Probe;
    public string? Comments;
    public MeasurementSource Source;
    public MeasurementResolution Resolution;
    public float GaindB;
    public bool DeepCoatOn;
    public bool HasAScan;
    public bool IsRadial;
    public MeasurementType Type;
    public GaugeAScan AScan;
    public GaugeEchoPoint[] EchoPoints = [];
    public uint BlankingTime;
    public ushort NumberOfEchoPoints;
    public int DepthCentimetres;
    public int SurfaceTemperatureCelsius;
    public uint PointIndex;
    public GaugeMeasurement[] Radials = [];
    public uint RecordId;
    public uint Key;
    public Method Method;
    public uint? Thickness;
}
