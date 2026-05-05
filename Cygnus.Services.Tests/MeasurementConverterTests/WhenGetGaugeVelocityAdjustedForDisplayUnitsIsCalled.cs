using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetGaugeVelocityAdjustedForDisplayUnitsIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 2540u, 1000u)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 5634u, 5634u)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 7567u, 7567u)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 2000u, 5080u)]
    public void CorrectValueShouldBeReturned(MeasurementUnits units, MeasurementUnits measurementUnits, uint velocity, uint expectedVelocity)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units);
        
        // Act
        var displayedVelocity = sut.GetMeasuredVelocityAdjustedForDisplayUnits(velocity, measurementUnits);

        // Assert
        displayedVelocity.ShouldBe(expectedVelocity);
    }
}
