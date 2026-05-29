namespace Cygnus.Models;

public record LiveMeasurement : Measurement
{    
    public bool ValidMeasurement;
    public bool StableMeasurement;
    public bool ProbeConnected;
    public bool DeepCoatAvailable;
    public bool ProbeZeroAvailable;
    public bool WaitingProbeZero;
    public MeasureMode[] MeasurementModesAvailable = [];
    public AScanFixedRange[] AScanRangesAvailable = [];

    public uint Index { get; set; }
    public uint BatteryLevel { get; set; }
    public bool IsFrozen { get; set; }
}
