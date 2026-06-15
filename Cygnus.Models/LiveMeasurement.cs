namespace Cygnus.Models;

public record LiveMeasurement : GaugeMeasurement
{    
    public bool ValidMeasurement;
    public bool StableMeasurement;
    public bool ProbeConnected;
    public bool DeepCoatAvailable;
    public bool ProbeZeroAvailable;
    public bool WaitingProbeZero;
    public MeasureMode[] MeasurementModesAvailable = [];
    public AScanFixedRange[] AScanRangesAvailable = [];
    public uint BatteryLevel;
    public bool IsFrozen;
}
