using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetDisplayedDepthFromCentimetresIsCalled
{
    [Test]
    [TestCase("155m", 15510, MeasurementUnits.Metric)]
    [TestCase("32m", 3190, MeasurementUnits.Metric)]
    [TestCase("0ft" , 0, MeasurementUnits.Imperial)]
    [TestCase("0m", 0, MeasurementUnits.Metric)]
    [TestCase("403ft" , 12291, MeasurementUnits.Imperial)]
    [TestCase("139ft", 4231, MeasurementUnits.Imperial)]
    public void CorrectValueShouldBeReturned(string displayedDepth, int depthCm, MeasurementUnits units)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units);

        // Act
        var result = sut.GetDisplayedDepth(depthCm);

        // Assert
        result.ShouldBe(displayedDepth);
    }
}
