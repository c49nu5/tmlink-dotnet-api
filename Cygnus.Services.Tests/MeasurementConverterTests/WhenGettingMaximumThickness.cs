using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGettingMaximumThickness
{
    [Test]
    [TestCase(MeasurementUnits.Metric, 1500)]
    [TestCase(MeasurementUnits.Imperial, 4)]

    public void CorrectValueShouldBeReturned(MeasurementUnits units, double expected)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        testBed.MeasurementSettingsService.Setup(m => m.Units).Returns(units);

        // Act
        var result = sut.MaximumThickness;

        // Assert
        result.ShouldBe(expected);
    }
}
