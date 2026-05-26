using Cygnus.Interfaces;
using Cygnus.Models;
using System.Text.RegularExpressions;

namespace Cygnus.Services;
internal partial class MeasurementConverter : IMeasurementConverter
{
    private readonly IMeasurementDisplaySettingsService _settingsService;

    public MeasurementConverter(IMeasurementDisplaySettingsService settingsService)
    {
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
    }

    public string GetDisplayedTemperature(int temperatureCelsius)
    {
        if (temperatureCelsius == 0)
        {
            return string.Empty;
        }

        if (_settingsService.Units == MeasurementUnits.Imperial)
        {
            return $"{(int)(temperatureCelsius * 1.8 + 32)}F";
        }

        return $"{temperatureCelsius}C";
    }

    public string GetDisplayedDepth(int depthCentimetres)
    {
        if (!double.IsNormal(depthCentimetres))
        {
            return _settingsService.Units == MeasurementUnits.Metric ?  "0m" : "0ft";
        }

        if (_settingsService.Units == MeasurementUnits.Imperial)
        {
            double depthInches = depthCentimetres / 2.54;
            int depthFeet = (int)Math.Round(depthInches / 12, MidpointRounding.AwayFromZero);
            return $"{depthFeet}ft";
        }

        int depthMetres = (int)Math.Round(depthCentimetres / 100.0, MidpointRounding.AwayFromZero);
        return $"{depthMetres}m";
    }

    public string GetDisplayedThicknessFromMillimetres(double thickness)
    {
        if (!double.IsNormal(thickness))
        {
            return string.Empty;
        }

        thickness = ConvertThickness(thickness, MeasurementUnits.Metric, _settingsService.Units);

        double roundedThickness;
        if (_settingsService.Units == MeasurementUnits.Imperial)
        {
            double multiple = _settingsService.Resolution switch
            {
                MeasurementResolution.Low => 0.005,
                MeasurementResolution.Medium => 0.002,
                _ => 0.001,
            };
            roundedThickness = Math.Round(thickness / multiple, MidpointRounding.AwayFromZero) * multiple;
        }
        else
        {
            roundedThickness = _settingsService.Resolution switch
            {
                MeasurementResolution.Low => Math.Round(thickness, 1),
                MeasurementResolution.Medium => Math.Round(thickness / 0.05, MidpointRounding.AwayFromZero) * 0.05,
                _ => Math.Round(thickness, 2),
            };
        }

        if (roundedThickness > 0)
        {
            return roundedThickness.ToString(ThicknessFormatString, Thread.CurrentThread.CurrentUICulture) + ThicknessString;
        }

        return string.Empty;
    }        

    public uint GetThicknessTimeFromDisplayedThickness(string displayedThickness, uint velocity, MeasurementUnits measurementUnits)
    {
        if (TryParseDisplayedThickness(displayedThickness, out double thickness))
        {
            return GetThicknessTimeFromDisplayedThickness(thickness, velocity, measurementUnits);
        }

        return Convert.ToUInt32(thickness);
    }

    public uint GetThicknessTimeFromDisplayedThickness(double thickness, uint velocity, MeasurementUnits measurementUnits)
    {
        thickness /= GetNsToThicknessMultiplier(GetMeasuredVelocityAdjustedForDisplayUnits(velocity, measurementUnits));
        return Convert.ToUInt32(thickness);
    }

    public double GetThicknessFromDisplayedThickness(string displayedThickness, MeasurementUnits measurementUnits)
    {
        if (TryParseDisplayedThickness(displayedThickness, out double thickness))
        {
            thickness = GetThicknessFromDisplayedValue(thickness, measurementUnits);
        }

        return thickness;
    }

    public double GetMillimetresForMeasurement(MeasurementUnits sourceUnits, uint thicknessTime, uint velocity)
    {
        double adjustedVelocity = velocity;
        if (sourceUnits != MeasurementUnits.Metric)
        {
            adjustedVelocity = GetMeasuredVelocityAdjustedForMeasurementUnits(velocity, sourceUnits, MeasurementUnits.Metric);
        }

        double thicknessMultiplier = GetNsToThicknessMultiplier(adjustedVelocity, MeasurementUnits.Metric);
        return thicknessTime * thicknessMultiplier;
    }

    public string GetDisplayedVelocity(uint velocity, bool includeUnits = true)
    {
        if (velocity == 0)
        {
            return string.Empty;
        }

        string displayedVelocity = _settingsService.Units == MeasurementUnits.Metric
            ? velocity.ToString("0")
            : (velocity / 1E4).ToString("0.0000");
        if (includeUnits)
        {
            return displayedVelocity + SpeedString;
        }

        return displayedVelocity;
    }

    public string GetDisplayedVelocity(uint measurementVelocity, MeasurementUnits measurementUnits, bool includeUnits = true)
    {
        uint adjustedVelocity = GetMeasuredVelocityAdjustedForDisplayUnits(measurementVelocity, measurementUnits);
        return GetDisplayedVelocity(adjustedVelocity, includeUnits);
    }

    public double GetDisplayedVelocity(double velocity, MeasurementUnits measurementUnits)
    {
        double displayedVelocity = GetMeasuredVelocityAdjustedForMeasurementUnits(velocity, measurementUnits, _settingsService.Units);
        displayedVelocity = _settingsService.Units == MeasurementUnits.Metric ? displayedVelocity : displayedVelocity / 1E4;
        return GetDisplayedVelocity(displayedVelocity);
    }

    public uint GetVelocity(string displayedVelocity)
    {
        if (double.TryParse(displayedVelocity, out double velocity))
        {
            return Convert.ToUInt32(_settingsService.Units == MeasurementUnits.Metric ? velocity : velocity * 1E4);
        }

        return 0;
    }

    public double GetMinVelocity()
    {
        return _settingsService.Units == MeasurementUnits.Metric ? Models.Constants.Velocities.MinimumMetric : Models.Constants.Velocities.MinimumImperial;
    }

    public double GetMaxVelocity()
    {
        return _settingsService.Units == MeasurementUnits.Metric ? Models.Constants.Velocities.MaximumMetric : Models.Constants.Velocities.MaximumImperial;
    }

    public uint GetVelocityForMaterialList(double velocity, MeasurementUnits Units)
    {
        return Convert.ToUInt32(Units == MeasurementUnits.Metric ? velocity : velocity / 1e4);
    }

    public uint GetMeasuredVelocityAdjustedForDisplayUnits(uint measurementVelocity, MeasurementUnits measurementUnits)
    {
        return Convert.ToUInt32(Math.Round(GetMeasuredVelocityAdjustedForMeasurementUnits(measurementVelocity, measurementUnits, _settingsService.Units)));
    }

    public double GetNsToThicknessMultiplier(double velocity) => GetNsToThicknessMultiplier(velocity, _settingsService.Units);

    public double GetNsToThicknessMultiplier(double velocity, MeasurementUnits measurementUnits) => velocity / GetDivisorForUnits(measurementUnits);

    public string GetDisplayedThickness(double thickness, bool includeUnits = true)
    {
        if (!double.IsNormal(thickness))
        {
            return string.Empty;
        }

        double roundedThickness;
        if (_settingsService.Units == MeasurementUnits.Metric)
        {
            roundedThickness = _settingsService.Resolution switch
            {
                MeasurementResolution.Low => Math.Round(thickness, 1),
                MeasurementResolution.Medium => Math.Round(thickness / 0.05, MidpointRounding.AwayFromZero) * 0.05,
                _ => Math.Round(thickness, 2),
            };
        }
        else
        {
            double multiple = _settingsService.Resolution switch
            {
                MeasurementResolution.Low => 0.005,
                MeasurementResolution.Medium => 0.002,
                _ => 0.001,
            };

            roundedThickness = Math.Round(thickness / multiple, MidpointRounding.AwayFromZero) * multiple;
        }

        if (roundedThickness > 0)
        {
            if (includeUnits)
            {
                return roundedThickness.ToString(ThicknessFormatString, Thread.CurrentThread.CurrentUICulture) + ThicknessString;
            }
            else
            {
                return roundedThickness.ToString(ThicknessFormatString, Thread.CurrentThread.CurrentUICulture);
            }
        }

        return string.Empty;
    }
    public double GetRoundedThickness(double thickness)
    {
        if (!double.IsNormal(thickness))
        {
            return 0;
        }

        double roundedThickness;
        if (_settingsService.Units == MeasurementUnits.Metric)
        {
            roundedThickness = _settingsService.Resolution switch
            {
                MeasurementResolution.Low => Math.Round(thickness, 1),
                MeasurementResolution.Medium => Math.Round(thickness / 0.05, MidpointRounding.AwayFromZero) * 0.05,
                _ => Math.Round(thickness, 2),
            };
        }
        else
        {
            double multiple = _settingsService.Resolution switch
            {
                MeasurementResolution.Low => 0.005,
                MeasurementResolution.Medium => 0.002,
                _ => 0.001,
            };

            roundedThickness = Math.Round(thickness / multiple, MidpointRounding.AwayFromZero) * multiple;
        }

        if (roundedThickness > 0)
        {

            return Math.Round(roundedThickness, ThicknessDecimalPlaces);

        }
        return 0;
    }

    public string GetDisplayedThickness(uint thickness, MeasurementUnits measurementUnits, bool includeUnits = true)
    {
        return GetDisplayedThickness(thickness / 1000.0, measurementUnits, includeUnits);
    }

    public string GetDisplayedThickness(double thickness, MeasurementUnits measurementUnits, bool includeUnits = true)
    {
        thickness = ConvertThickness(thickness, measurementUnits, _settingsService.Units);
        return GetDisplayedThickness(thickness, includeUnits);
    }

    public string GetDisplayedThickness(uint thicknessTime, uint measurementVelocity, MeasurementUnits measurementUnits, bool includeUnits = true)
    {
        double displayedThicknessValue = GetDisplayedThicknessValue(thicknessTime, measurementVelocity, measurementUnits);
        return GetDisplayedThickness(displayedThicknessValue, includeUnits);
    }

    public double GetDisplayedThicknessValue(uint thicknessTime, uint measurementVelocity, MeasurementUnits measurementUnits)
    {
        double adjustedVelocity = GetMeasuredVelocityAdjustedForMeasurementUnits(measurementVelocity, measurementUnits, _settingsService.Units);
        double thicknessMultiplier = GetNsToThicknessMultiplier(adjustedVelocity);
        double displayedThicknessValue = thicknessTime * thicknessMultiplier;
        return displayedThicknessValue;
    }

    public double GetDisplayedThicknessValue(double thickness, MeasurementUnits measurementUnits)
    {
        return ConvertThickness(thickness, measurementUnits, _settingsService.Units);
    }

    /// <summary>
    /// The thickness format string ensures that thicknesses are presented consistently with the correct number of decimal places for the display resolution
    /// </summary>
    public string ThicknessFormatString => $"F{ThicknessDecimalPlaces}";

    public int ThicknessDecimalPlaces
    {
        get
        {
            int decimalPlaces = 3;
            if (_settingsService.Units == MeasurementUnits.Metric)
            {
                decimalPlaces = _settingsService.Resolution == MeasurementResolution.Low ? 1 : 2;
            }

            return decimalPlaces;
        }
    }

    /// <summary>
    /// The chart label format string ensures that the thickness only take up the minimum width in the chart
    /// i.e.
    /// no trailing 0, 1.00 will be displayed as 1 when in metric units
    /// no excesive decimal places on the cursors, 1.1231234 will be displayed as 1.12 in metric units
    /// </summary>
    public string ChartLabelFormatString => _settingsService.Units == MeasurementUnits.Metric ? "0.##" : "0.0##";

    /// <summary>
    /// If the measurement was taken in different units to the current display units then we need to adjust the velocity
    /// This private implementation gives a more accurate double, rather than the rounded response
    /// </summary>
    public double GetMeasuredVelocityAdjustedForMeasurementUnits(double measurementVelocity, MeasurementUnits sourceUnits, MeasurementUnits destinationUnits)
    {
        double velocity = measurementVelocity;
        if (sourceUnits == MeasurementUnits.Metric && destinationUnits == MeasurementUnits.Imperial)
        {
            velocity /= 2.54;
        }
        else if (sourceUnits != MeasurementUnits.Metric && destinationUnits == MeasurementUnits.Metric)
        {
            velocity *= 2.54;
        }

        return velocity;
    }

    private double GetDisplayedVelocity(double displayedVelocity)
    {
        int decimalPlaces = 4;
        if (_settingsService.Units == MeasurementUnits.Metric)
        {
            decimalPlaces = 0;
        }

        displayedVelocity = Math.Round(displayedVelocity, decimalPlaces);
        return Math.Clamp(displayedVelocity, GetMinVelocity(), GetMaxVelocity());
    }


    public double GetThicknessFromDisplayedValue(double thickness, MeasurementUnits sourceUnits)
    {
        return ConvertThickness(thickness, _settingsService.Units, sourceUnits);
    }

    public double ConvertThickness(double thickness, MeasurementUnits sourceUnits, MeasurementUnits destinationUnits)
    {
        if (sourceUnits == MeasurementUnits.Metric && destinationUnits == MeasurementUnits.Imperial)
        {
            thickness /= 25.4;
        }
        else if (sourceUnits != MeasurementUnits.Metric && destinationUnits == MeasurementUnits.Metric)
        {
            thickness *= 25.4;
        }

        return thickness;
    }

    public double ConvertThickness(double thickness, MeasurementUnits sourceUnits)
    {
        if (sourceUnits == MeasurementUnits.Metric && _settingsService.Units == MeasurementUnits.Imperial)
        {
            thickness /= 25.4;
        }
        else if (sourceUnits != MeasurementUnits.Metric && _settingsService.Units == MeasurementUnits.Metric)
        {
            thickness *= 25.4;
        }

        return thickness;
    }

    public uint GetTargetVelocityForThicknessTime(double targetThickness, uint calibrationThicknessTime)
    {
        uint velocity = (uint)(targetThickness * GetDivisorForUnits(_settingsService.Units) / calibrationThicknessTime);
        velocity = _settingsService.Units == MeasurementUnits.Metric ?
           (uint)Math.Clamp(velocity, GetMinVelocity(), GetMaxVelocity()) :
           (uint)Math.Clamp(velocity, GetMinVelocity() * 1e4, GetMaxVelocity() * 1e4);
        return velocity;
    }

    private static double GetDivisorForUnits(MeasurementUnits measurementUnits)
    {
        return (double)(measurementUnits == MeasurementUnits.Imperial ? 2e7 : 2e6);
    }

    public double GetThicknessIncrement()
    {
        if (_settingsService.Units == MeasurementUnits.Metric)
        {
            return _settingsService.Resolution switch
            {
                MeasurementResolution.Low => 0.1,
                MeasurementResolution.Medium => 0.05,
                MeasurementResolution.High => 0.01,
                _ => 0.05,
            };
        }
        else
        {
            return _settingsService.Resolution switch
            {
                MeasurementResolution.Low => 0.005,
                MeasurementResolution.Medium => 0.002,
                MeasurementResolution.High => 0.001,
                _ => 0.002,
            };
        }
    }

    public double GetVelocityIncrement() => _settingsService.Units == MeasurementUnits.Metric ? 1 : 0.0001;

    public double? GetWastage(string displayedThickness, string displayedReference, string displayedMinimum, string? parentReference, string? parentMinimum)
    {
        if (displayedReference == string.Empty) displayedReference = parentReference ?? string.Empty;
        if (displayedMinimum == string.Empty) displayedMinimum = parentMinimum ?? string.Empty;

        if (TryParseDisplayedThickness(displayedThickness, out double thickness))
        {
            if (TryParseDisplayedThickness(displayedMinimum, out double minimumThickness) && TryParseDisplayedThickness(displayedReference, out double referenceThickness))
            {
                if (minimumThickness == referenceThickness) return 0;
                double percentage = (thickness - referenceThickness) / (minimumThickness - referenceThickness);
                return percentage;
            }
        }

        return null;
    }

    public string GetRelayThickness(uint thicknessTime, uint velocity, MeasurementUnits units)
    {
        if (thicknessTime == 0)
        {
            return new string(' ', 6);
        }

        double thicknessMultiplier = GetNsToThicknessMultiplier(velocity, units);
        double displayedThicknessValue = thicknessTime * thicknessMultiplier;
        return displayedThicknessValue.ToString(units == MeasurementUnits.Metric ? "000.00" : "00.000");
    }

    public string GetRelayVelocity(uint velocity, MeasurementUnits units)
    {
        return units == MeasurementUnits.Imperial ?
            (velocity * 0.0001d).ToString("F4") :
            velocity.ToString("0000");
    }

    private static bool TryParseDisplayedThickness(string displayedThickness, out double thickness)
    {
        if (displayedThickness != null)
        {
            Match numericMatch = NumericParser.Match(displayedThickness);
            if (numericMatch.Success)
            {
                return double.TryParse(numericMatch.Value, out thickness);
            }
        }

        thickness = 0.0;
        return false;
    }

    public double GetMaximumThickness => _settingsService.Units == MeasurementUnits.Metric ? Models.Constants.Measurements.MaxMeasurementThicknessMetric : Models.Constants.Measurements.MaxMeasurementThicknessImperial;

    public string ThicknessString => _settingsService.Units == MeasurementUnits.Metric ? "mm" : "in";

    public string SpeedString => _settingsService.Units == MeasurementUnits.Metric ? "m/s" : "in/µs";

    private static Regex NumericParser => new Regex(@"^-?\d+(?:[\,\.]\d+)?");
}
