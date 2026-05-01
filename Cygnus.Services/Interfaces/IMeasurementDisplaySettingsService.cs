using Cygnus.Models;

namespace Cygnus.Services.Interfaces;

/// <summary>
/// Settings that affect the display of the measurements
/// </summary>
public interface IMeasurementDisplaySettingsService
{
    MeasurementUnits Units { get; set; }
    MeasurementResolution Resolution { get; set; }
}
