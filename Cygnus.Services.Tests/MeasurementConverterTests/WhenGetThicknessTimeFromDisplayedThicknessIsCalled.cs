using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetThicknessTimeFromDisplayedThicknessIsCalled
{
    [Test]
    [TestCase("15.5", 5634u, MeasurementUnits.Metric, 5502u)]
    [TestCase("3.2", 5920u, MeasurementUnits.Metric, 1081u)]
    [TestCase("0.5940" , 7567u, MeasurementUnits.Imperial, 1570u)]
    [TestCase("1.230", 6200u, MeasurementUnits.Imperial, 3968u)]
    public void CorrectValueShouldBeReturned(string displayedThickness, double velocity, MeasurementUnits units, uint expected)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units);

        // Act
        var result = sut.GetThicknessTimeFromDisplayedThickness(displayedThickness, (uint)velocity, units);

        // Assert
        result.ShouldBe(expected);
    }

    [Test]
    [TestCase(15.5, 5634u, MeasurementUnits.Metric, 5502u)]
    [TestCase(3.2, 5920u, MeasurementUnits.Metric, 1081u)]
    [TestCase(0.5940, 7567u, MeasurementUnits.Imperial, 1570u)]
    [TestCase(1.230, 6200u, MeasurementUnits.Imperial, 3968u)]
    public void CorrectValueShouldBeReturned(double displayedThickness, double velocity, MeasurementUnits units, uint expected)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        testBed.MeasurementSettingsService.Setup(o => o.Units).Returns(units);
        // Act
        var result = sut.GetThicknessTimeFromDisplayedThickness(displayedThickness, (uint)velocity, units);

        // Assert
        result.ShouldBe(expected);
    }
}
