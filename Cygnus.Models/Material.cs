namespace Cygnus.Models;

public class Material
{
    public string Name { get; set; } = string.Empty;
    public uint Velocity { get; set; }
    public MeasurementUnits MeasurementUnits { get; set; }
}
