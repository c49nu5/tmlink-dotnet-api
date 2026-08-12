namespace Cygnus.Models;
public record struct GaugeEchoPoint
{
    public uint Time;
    public short Level;

    public uint Thickness { get; set; }
}
