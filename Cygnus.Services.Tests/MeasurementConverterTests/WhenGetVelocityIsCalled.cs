using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;

internal class WhenGetVelocityIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, 8547u, "0.8547")]
    [TestCase(MeasurementUnits.Metric, 4124u, "4124")]
    [TestCase(MeasurementUnits.Imperial, 0u, "")]
    public void CorrectValueShouldBeReturned(MeasurementUnits units, uint expectedVelocity, string velocity)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units);

        // Act
        var result = sut.GetVelocity(velocity);

        // Assert
        result.ShouldBe(expectedVelocity);
    }
}
