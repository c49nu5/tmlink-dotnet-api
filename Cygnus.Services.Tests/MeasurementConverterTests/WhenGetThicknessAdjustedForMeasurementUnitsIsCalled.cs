using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetThicknessAdjustedForMeasurementUnitsIsCalled
{
    [Test]
    [TestCase(10.14343, 10.14343, MeasurementUnits.Metric, MeasurementUnits.Metric)]
    [TestCase(0.119342, 0.119342, MeasurementUnits.Metric, MeasurementUnits.Metric)]
    [TestCase(10.14324, 10.14324, MeasurementUnits.Metric, MeasurementUnits.Metric)]
    [TestCase(3.03276, 0.1194, MeasurementUnits.Imperial, MeasurementUnits.Metric)]
    [TestCase(25.76383722, 1.0143243, MeasurementUnits.Imperial, MeasurementUnits.Metric)]
    [TestCase(3.0308042, 0.119323, MeasurementUnits.Imperial, MeasurementUnits.Metric)]
    [TestCase(1.004456692913386 , 25.5132, MeasurementUnits.Metric, MeasurementUnits.Imperial)]
    [TestCase(0.06506299212598426, 1.6526, MeasurementUnits.Metric, MeasurementUnits.Imperial)]
    [TestCase(1.004503937007874 , 25.5144, MeasurementUnits.Metric, MeasurementUnits.Imperial)]
    [TestCase(1.652342, 1.652342, MeasurementUnits.Imperial, MeasurementUnits.Imperial)]
    [TestCase(2.5512342 , 2.5512342, MeasurementUnits.Imperial, MeasurementUnits.Imperial)]
    [TestCase(1.65234, 1.65234, MeasurementUnits.Imperial, MeasurementUnits.Imperial)]
    public void CorrectValueShouldBeReturned(double adjustedThickness, double thickness, MeasurementUnits units, MeasurementUnits measurementUnits)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units);

        // Act
        var result = sut.GetThicknessFromDisplayedValue(thickness, measurementUnits);

        // Assert
        result.ShouldBe(adjustedThickness);
    }
}
