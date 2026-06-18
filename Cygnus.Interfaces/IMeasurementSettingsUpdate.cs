using Cygnus.Models;

namespace Cygnus.Interfaces;

public interface IMeasurementSettingsUpdate
{
    MeasureMode MeasureMode { get; }
    AScanFixedRange AScanRange { get; }
    ProbeType ProbeType { get; }
    bool ManualGain { get; set; }
    int GaindB { get; set; }
    uint Velocity { get; set; }
    bool DeepCoatOn { get; set; }
    bool AutoDetectProbeOn { get; set; }
}
