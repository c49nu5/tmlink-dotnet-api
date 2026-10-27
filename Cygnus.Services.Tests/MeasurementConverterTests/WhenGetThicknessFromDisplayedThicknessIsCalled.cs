using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;

internal class WhenGetThicknessFromDisplayedThicknessIsCalled
{
    [Test]
    [TestCase("15.52876", MeasurementUnits.Metric, MeasurementUnits.Metric, 15.52876)]
    [TestCase("3.27865", MeasurementUnits.Metric, MeasurementUnits.Metric, 3.27865)]
    [TestCase("0.5948380", MeasurementUnits.Imperial, MeasurementUnits.Metric, 15.1088)]
    [TestCase("1.25765", MeasurementUnits.Imperial, MeasurementUnits.Metric, 31.9443)]
    [TestCase("sdf230", MeasurementUnits.Imperial, MeasurementUnits.Metric, 0u)]
    [TestCase("15.52876", MeasurementUnits.Metric, MeasurementUnits.Imperial, 0.6113685)]
    [TestCase("3.27865", MeasurementUnits.Metric, MeasurementUnits.Imperial, 0.12908)]
    [TestCase("0.5948380", MeasurementUnits.Imperial, MeasurementUnits.Imperial, 0.5948380)]
    [TestCase("1.25765", MeasurementUnits.Imperial, MeasurementUnits.Imperial, 1.25765)]
    [TestCase("sdf230", MeasurementUnits.Imperial, MeasurementUnits.Imperial, 0u)]
    public void WithoutVelocity_CorrectValueShouldBeReturned(string displayedThickness, MeasurementUnits displayedUnits, MeasurementUnits units, double expected)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(displayedUnits);

        // Act
        var result = sut.GetThicknessFromDisplayedThickness(displayedThickness, units);

        // Assert
        result.ShouldBe(expected, 0.001);
    }
}
