using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetDisplayedTemperatureCompensationIsCalled
{
    [Test]
    [TestCase("10C", 10, MeasurementUnits.Metric)]
    [TestCase("119C", 119, MeasurementUnits.Metric)]
    [TestCase("", 0, MeasurementUnits.Metric)]
    [TestCase("" , 0, MeasurementUnits.Imperial)]
    [TestCase("195F" , 91, MeasurementUnits.Imperial)]
    [TestCase("33F", 1, MeasurementUnits.Imperial)]
    public void CorrectValueShouldBeReturned(string displayedTempComp, int tempComp, MeasurementUnits units)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units);

        // Act
        var result = sut.GetDisplayedTemperature(tempComp);

        // Assert
        result.ShouldBe(displayedTempComp);
    }
}
