using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetRelayThicknessIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, 12190u, 200u, "00.122")]
    [TestCase(MeasurementUnits.Imperial, 12220u, 200u, "00.122")]
    [TestCase(MeasurementUnits.Imperial, 12370u, 200u, "00.124")]
    [TestCase(MeasurementUnits.Imperial, 126800u, 20u, "00.127")]
    [TestCase(MeasurementUnits.Imperial, 131900u, 20u, "00.132")]
    [TestCase(MeasurementUnits.Imperial, 135900u, 20u, "00.136")]
    [TestCase(MeasurementUnits.Imperial, 1219u, 2000u, "00.122")]
    [TestCase(MeasurementUnits.Imperial, 1222u, 2000u, "00.122")]
    [TestCase(MeasurementUnits.Imperial, 1239u, 2000u, "00.124")]
    [TestCase(MeasurementUnits.Imperial, 1269u, 2000u, "00.127")]
    [TestCase(MeasurementUnits.Imperial, 1319u, 2000u, "00.132")]
    [TestCase(MeasurementUnits.Imperial, 1359u, 2000u, "00.136")]
    [TestCase(MeasurementUnits.Imperial, 1233u, 2000u, "00.123")]
    [TestCase(MeasurementUnits.Imperial, 12350u, 200u, "00.124")]
    [TestCase(MeasurementUnits.Imperial, 123600u, 20u, "00.124")]
    [TestCase(MeasurementUnits.Metric, 14231u, 200u, "001.42")]
    [TestCase(MeasurementUnits.Metric, 14531u, 200u, "001.45")]
    [TestCase(MeasurementUnits.Metric, 14731u, 200u, "001.47")]
    [TestCase(MeasurementUnits.Metric, 15131u, 200u, "001.51")]
    [TestCase(MeasurementUnits.Metric, 53124u, 200u, "005.31")]
    [TestCase(MeasurementUnits.Metric, 53224u, 200u, "005.32")]
    [TestCase(MeasurementUnits.Metric, 53284u, 200u, "005.33")]
    [TestCase(MeasurementUnits.Metric, 53424u, 200u, "005.34")]
    [TestCase(MeasurementUnits.Metric, 53724u, 200u, "005.37")]
    [TestCase(MeasurementUnits.Metric, 53124u, 200u, "005.31")]
    [TestCase(MeasurementUnits.Metric, 53224u, 200u, "005.32")]
    [TestCase(MeasurementUnits.Metric, 53724u, 200u, "005.37")]
    [TestCase(MeasurementUnits.Metric, 53784u, 200u, "005.38")]
    [TestCase(MeasurementUnits.Metric, 53824u, 200u, "005.38")]
    [TestCase(MeasurementUnits.Metric, 53924u, 200u, "005.39")]
    [TestCase(MeasurementUnits.Metric, 0u, 7654u, "      ")]
    [TestCase(MeasurementUnits.Metric, 0u, 6540u, "      ")]
    public void WithDifferingMeasurementUnits_CorrectValueShouldBeReturned(MeasurementUnits units, uint thickness, uint velocity, string expectedThickness)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        
        // Act
        var displayedThickness = sut.GetRelayThickness(thickness, velocity, units);

        // Assert
        displayedThickness.ShouldBe(expectedThickness);
    }
}
