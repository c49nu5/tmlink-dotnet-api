using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;

internal class WhenGetDisplayedVelocityValueIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 12547, 0.3543)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 14124, 9000)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 13365, 9000)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 16475, 0.3543)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 2547, 0.2547)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 4124, 4124)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 3365, 8547)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 6475, 0.2549)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 0, 0.0394)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 0, 1000)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 0, 1000)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 0, 0.0394)]
    public void WithDifferingMeasurementUnits_CorrectValueShouldBeReturned(MeasurementUnits displayUnits, MeasurementUnits measurementUnits, double velocity, double expectedVelocity)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(displayUnits);

        // Act
        var displayedVelocity = sut.GetDisplayedVelocityValue(velocity, measurementUnits);

        // Assert
        displayedVelocity.ShouldBe(expectedVelocity);
    }
}
