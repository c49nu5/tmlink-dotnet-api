using MessagePack;

namespace Cygnus.Models;

[MessagePackObject]
public record Measurement
{
    [Key(0)]
    public Guid Id;
    [Key(1)]
    public string Name = string.Empty;
    [Key(2)]
    public GridCoordinate GridCoordinate;
    [Key(3)]
    public uint ThicknessTime;
    [Key(4)]
    public MeasurementUnits Units;
    [Key(5)]
    public DateTimeOffset? Time;
    [Key(6)]
    public MeasureMode Mode;
    [Key(7)]
    public uint Velocity;
    [Key(8)]
    public double ReferenceThickness;
    [Key(9)]
    public double MinimumThickness;
    [Key(10)]
    public ProbeType Probe;
    [Key(11)]
    public string? Comments;
    [Key(12)]
    public MeasurementSource Source;
    [Key(13)]
    public MeasurementResolution Resolution;
    [Key(14)]
    public float GaindB;
    [Key(15)]
    public bool DeepCoatOn;
    [Key(16)]
    public bool HasAScan;
    [Key(17)]
    public bool IsRadial;
    [Key(18)]
    public MeasurementType Type;
    [Key(19)]
    public AScan AScan;
    [Key(20)]
    public EchoPoint[] EchoPoints = [];
    [Key(21)]
    public uint BlankingTime;
    [Key(22)]
    public ushort NumberOfEchoPoints;
    [Key(23)]
    public int DepthCentimetres;
    [Key(24)]
    public int SurfaceTemperatureCelsius;
    [Key(25)]
    public int PointIndex;
    [Key(26)]
    public Journal? Journal;
    [Key(27)]
    public Measurement[] Radials = [];
    [Key(28)]
    public uint RecordId { get; set; }
    [Key(29)]
    public uint Key { get; set; }
    [Key(30)]
    public Method Method { get; set; }
    [Key(31)]
    public uint? Thickness { get; set; }
}
