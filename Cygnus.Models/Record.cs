using MessagePack;
namespace Cygnus.Models;

[MessagePackObject]
public record Record : MeasurementGroup
{
    [Key(10)]
    public GaugeType GaugeType;
    [Key(11)]
    public uint GaugeId;
    [Key(12)]
    public uint SerialNumber;
    [Key(13)]
    public string Title = string.Empty;
    [Key(14)]
    public string[] Comments = [];
    [Key(15)]
    public string Location = string.Empty;
    [Key(16)]
    public string Surveyor = string.Empty;
    [Key(17)]
    public double ReferenceThickness;
    [Key(18)]
    public double MinimumThickness;
}
