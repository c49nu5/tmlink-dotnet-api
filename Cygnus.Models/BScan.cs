using MessagePack;

namespace Cygnus.Models;

[MessagePackObject]
public record BScan : Record
{
    [Key(20)]
    public double ThicknessRange;
    [Key(21)]
    public int MinThicknessTime;
    [Key(22)]
    public double ScanLength;
    [Key(23)]
    public int ScanDuration;
    [Key(24)]
    public int ScanInterval;
    [Key(25)]
    public ScanDirection Direction;
}
