using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;

internal class WhenConvertVelocityIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 12547u, 12547u)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 14124u, 14124u)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 13365u, 33947u)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 16475u, 6486u)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 2547u, 2547u)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 4124u, 4124u)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 3365u, 8547u)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 6475u, 2549u)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 0u, 0u)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 0u, 0u)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 0u, 0u)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 0u, 0u)]
    public void WithDifferingMeasurementUnits_CorrectValueShouldBeReturned(MeasurementUnits displayUnits, MeasurementUnits measurementUnits, uint velocity, uint expectedVelocity)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        var calibratedVelocity = new CalibratedVelocity { Velocity = velocity, MeasurementUnits = measurementUnits };

        // Act
        var displayedVelocity = sut.ConvertVelocity(calibratedVelocity, displayUnits);

        // Assert
        displayedVelocity.ShouldBe(expectedVelocity);
    }
}
