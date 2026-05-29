namespace Cygnus.Models;

public enum RecordType
{
    None = 0,
    Linear = 1,         // 1D List of measurements.
    Grid2D = 2,         // 2D array of measurements.
    Multipoint = 3,     // 3D array of measurements.
    BScan = 4,
    Journal = 5,
}
