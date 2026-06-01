using MessagePack;

namespace Cygnus.Models;

[MessagePackObject]
public record struct GridCoordinate
{
    [Key(0)]
    public ushort Column;
    [Key(1)]
    public ushort Row;
    [Key(2)]
    public ushort Point;
    [Key(3)]
    public ushort RadialNumber;
}
