using Cygnus.Models;
using System.Text.RegularExpressions;

namespace Cygnus.Interfaces;

public interface IMeasurementConverter
{
    string ChartLabelFormatString { get; }
    int ThicknessDecimalPlaces { get; }
    string ThicknessFormatString { get; }
    string GetDisplayedThickness(double thickness, bool includeUnits = true);
    double GetRoundedThickness(double thickness);
    string GetDisplayedThickness(uint thickness, MeasurementUnits measurementUnits, bool includeUnits = true);
    string GetDisplayedThickness(double thickness, MeasurementUnits measurementUnits, bool includeUnits = true);
    string GetDisplayedThickness(uint thicknessTime, uint measurementVelocity, MeasurementUnits measurementUnits, bool includeUnits = true);
    string GetDisplayedVelocity(uint velocity, bool includeUnits = true);
    string GetDisplayedVelocity(uint measurementVelocity, MeasurementUnits measurementUnits, bool includeUnits = true);
    double GetDisplayedVelocity(double displayedVelocity, MeasurementUnits measurementUnits);
    uint GetVelocity(string value);
    uint GetMeasuredVelocityAdjustedForDisplayUnits(uint measuredVelocity, MeasurementUnits measurementUnits);
    double GetMeasuredVelocityAdjustedForMeasurementUnits(double measurementVelocity, MeasurementUnits sourceUnits, MeasurementUnits destinationUnits);
    double GetNsToThicknessMultiplier(double velocity);
    string GetDisplayedThicknessFromMillimetres(double thickness);
    double GetMillimetresForMeasurement(MeasurementUnits sourceUnits, uint thicknessTime, uint velocity);
    uint GetThicknessTimeFromDisplayedThickness(string displayedThickness, uint velocity, MeasurementUnits measurementUnits);
    uint GetThicknessTimeFromDisplayedThickness(double displayedThickness, uint velocity, MeasurementUnits measurementUnits);
    double GetNsToThicknessMultiplier(double velocity, MeasurementUnits measurementUnits);
    string GetDisplayedTemperature(int temperatureInCelsius);
    string GetDisplayedDepth(int depthCentimetres);
    double GetDisplayedThicknessValue(uint thicknessTime, uint measurementVelocity, MeasurementUnits measurementUnits);
    double GetDisplayedThicknessValue(double thicknessTime, MeasurementUnits measurementUnits);
    double ConvertThickness(double thickness, MeasurementUnits sourceUnits, MeasurementUnits destinationUnits);
    double ConvertThickness(double thickness, MeasurementUnits sourceUnits);
    double GetThicknessFromDisplayedValue(double thickness, MeasurementUnits sourceUnits);
    double GetThicknessIncrement();
    double? GetWastage(string displayedThickness, string displayedReference, string displayedMinimum, string? parentReference, string? parentMinimum);
    uint GetVelocityForMaterialList(double velocity, MeasurementUnits units);
    string GetRelayThickness(uint thicknessTime, uint velocity, MeasurementUnits units);
    string GetRelayVelocity(uint velocity, MeasurementUnits units);
    double GetThicknessFromDisplayedThickness(string value, MeasurementUnits measurementUnits);
    double GetVelocityIncrement();
    uint GetTargetVelocityForThicknessTime(double targetThickness, uint calibrationThicknessTime);
    double GetMinVelocity();
    double GetMaxVelocity();
    double GetMaximumThickness { get; }
}
