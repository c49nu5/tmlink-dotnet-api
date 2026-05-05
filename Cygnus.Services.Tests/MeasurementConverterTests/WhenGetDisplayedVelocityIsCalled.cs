using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetDisplayedVelocityIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, 8547u, "0.8547")]
    [TestCase(MeasurementUnits.Metric, 4124u, "4124")]
    [TestCase(MeasurementUnits.Imperial, 0u, "")]
    public void CorrectValueShouldBeReturned(MeasurementUnits units, uint velocity, string expectedVelocity)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units);
        
        // Act
        var displayedVelocity = sut.GetDisplayedVelocity(velocity, false);

        // Assert
        displayedVelocity.ShouldBe(expectedVelocity);
    }

    [Test]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 8547u, "0.8547")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 4124u, "4124")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 3365u, "8547")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 10475u, "0.4124")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 0u, "")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 0u, "")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 0u, "")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 0u, "")]
    public void WithDifferingMeasurementUnits_CorrectValueShouldBeReturned(MeasurementUnits displayUnits, MeasurementUnits measurementUnits, uint velocity, string expectedVelocity)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(displayUnits);
        
        // Act
        var displayedVelocity = sut.GetDisplayedVelocity(velocity, measurementUnits, false);

        // Assert
        displayedVelocity.ShouldBe(expectedVelocity);
    }
}
