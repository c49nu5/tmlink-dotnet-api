using MessagePack;

namespace Cygnus.Models;

[MessagePackObject]
public record Journal : MeasurementGroup
{
    [Key(10)]
    public double ThicknessRange;
}
