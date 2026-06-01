using MessagePack;

namespace Cygnus.Models;

[MessagePackObject]
public record struct EchoPoint
{
    [Key(0)]
    public uint Time;
    [Key(1)]
    public short Level;
}
