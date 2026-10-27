using Cygnus.Models;

namespace Cygnus.Interfaces;

public interface IMeasurementConverter
{
    string ChartLabelFormatString { get; }
    string ThicknessFormatString { get; }
    int ThicknessDecimalPlaces { get; }
    double MaximumThickness { get; }
    double ThicknessIncrement { get; }
    double VelocityIncrement { get; }
    double MinVelocity { get; }
    double MaxVelocity { get; }

    double ConvertThickness(double thickness, MeasurementUnits sourceUnits, MeasurementUnits destinationUnits);
    double ConvertThickness(double thickness, MeasurementUnits sourceUnits);
    uint ConvertVelocity(CalibratedVelocity calibratedVelocity, MeasurementUnits destinationUnits);
    string GetDisplayedDepth(int depthCentimetres);
    string GetDisplayedTemperature(int temperatureInCelsius);
    string GetDisplayedThickness(double thickness, bool includeUnits = true);
    string GetDisplayedThickness(uint thickness, MeasurementUnits measurementUnits, bool includeUnits = true);
    string GetDisplayedThickness(double thickness, MeasurementUnits measurementUnits, bool includeUnits = true);
    string GetDisplayedThickness(uint thicknessTime, uint measurementVelocity, MeasurementUnits measurementUnits, bool includeUnits = true);
    string GetDisplayedThicknessFromMillimetres(double thickness);
    double GetDisplayedThicknessValue(uint thicknessTime, uint measurementVelocity, MeasurementUnits measurementUnits);
    string GetDisplayedVelocity(uint velocity, bool includeUnits = true);
    string GetDisplayedVelocity(uint measurementVelocity, MeasurementUnits measurementUnits, bool includeUnits = true);
    double GetDisplayedVelocityValue(double displayedVelocity, MeasurementUnits measurementUnits);
    uint GetMeasuredVelocityAdjustedForDisplayUnits(uint measuredVelocity, MeasurementUnits measurementUnits);
    double GetMeasuredVelocityAdjustedForMeasurementUnits(double measurementVelocity, MeasurementUnits sourceUnits, MeasurementUnits destinationUnits);
    double GetMillimetresForMeasurement(MeasurementUnits sourceUnits, uint thicknessTime, uint velocity);
    double GetNsToThicknessMultiplier(double velocity);
    double GetNsToThicknessMultiplier(double velocity, MeasurementUnits measurementUnits);
    string GetRelayThickness(uint thicknessTime, uint velocity, MeasurementUnits units);
    string GetRelayVelocity(uint velocity, MeasurementUnits units);
    double GetRoundedThickness(double thickness);
    uint GetTargetVelocityForThicknessTime(double targetThickness, uint calibrationThicknessTime);
    uint GetThicknessTimeFromDisplayedThickness(string displayedThickness, uint velocity, MeasurementUnits measurementUnits);
    uint GetThicknessTimeFromDisplayedThickness(double displayedThickness, uint velocity, MeasurementUnits measurementUnits);
    double GetThicknessFromDisplayedThickness(string value, MeasurementUnits measurementUnits);
    double GetThicknessFromDisplayedValue(double thickness, MeasurementUnits sourceUnits);
    uint GetVelocity(string value);
    double? GetWastage(string displayedThickness, string displayedReference, string displayedMinimum, string? parentReference, string? parentMinimum);
}
