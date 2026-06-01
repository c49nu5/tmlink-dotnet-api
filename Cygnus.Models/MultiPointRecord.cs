using MessagePack;

namespace Cygnus.Models;

[MessagePackObject]
public record MultiPointRecord : GridRecord
{
    [Key(26)]
    public int PointCount;
    [Key(27)]
    public string PointNamePrefix = string.Empty;
}
