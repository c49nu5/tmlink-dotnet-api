using MessagePack;
namespace Cygnus.Models;

[MessagePackObject]
public record MeasurementGroup
{
    [Key(0)]
    public Guid Id;
    [Key(1)]
    public string Name = string.Empty;
    [Key(2)]
    public RecordType RecordType;
    [Key(3)]
    public string Notes = string.Empty;
    [Key(4)]
    public DateTimeOffset? Date;
    [Key(5)]
    public MeasurementUnits Units;
    [Key(6)]
    public Measurement[] Measurements = [];
    [Key(7)]
    public RecordCreator Creator;
    [Key(8)]
    public ProtectionState ProtectionState;
}
