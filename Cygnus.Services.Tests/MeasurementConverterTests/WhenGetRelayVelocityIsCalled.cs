using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetRelayVelocityIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, 8547u, "0.8547")]
    [TestCase(MeasurementUnits.Metric, 4124u, "4124")]
    [TestCase(MeasurementUnits.Imperial, 3365u, "0.3365")]
    [TestCase(MeasurementUnits.Metric, 10475u, "10475")]
    [TestCase(MeasurementUnits.Imperial, 0u, "0.0000")]
    [TestCase(MeasurementUnits.Metric, 0u, "0000")]
    [TestCase(MeasurementUnits.Imperial, 0u, "0.0000")]
    [TestCase(MeasurementUnits.Metric, 0u, "0000")]
    public void WithDifferingMeasurementUnits_CorrectValueShouldBeReturned(MeasurementUnits measurementUnits, uint velocity, string expectedVelocity)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        
        // Act
        var displayedVelocity = sut.GetRelayVelocity(velocity, measurementUnits);

        // Assert
        displayedVelocity.ShouldBe(expectedVelocity);
    }
}
