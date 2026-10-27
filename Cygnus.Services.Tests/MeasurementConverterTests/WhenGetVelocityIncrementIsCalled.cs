using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetVelocityIncrementIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Metric, 1)]
    [TestCase(MeasurementUnits.Imperial, .0001)]

    public void CorrectValueShouldBeReturned(MeasurementUnits units, double expected)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        testBed.MeasurementSettingsService.Setup(m => m.Units).Returns(units);

        // Act
        var result = sut.VelocityIncrement;

        // Assert
        result.ShouldBe(expected);
    }
}
