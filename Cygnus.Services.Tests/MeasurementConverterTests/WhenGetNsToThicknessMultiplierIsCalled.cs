using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetNsToThicknessMultiplierIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, 5634u, 0.0002817d)]
    [TestCase(MeasurementUnits.Metric, 7567u, 0.0037835d)]
    public void CorrectValueShouldBeReturned(MeasurementUnits units, uint velocity, double expectedMultiplier)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units);
        
        // Act
        var multiplier = sut.GetNsToThicknessMultiplier(velocity);

        // Assert
        multiplier.ShouldBe(expectedMultiplier);
    }
}
