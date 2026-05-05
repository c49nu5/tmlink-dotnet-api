using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetDisplayedThicknessIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Low, 0.1219, "0.120")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Low, 0.1222, "0.120")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Low, 0.1237, "0.125")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Low, 0.1268, "0.125")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Low, 0.1319, "0.130")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Low, 0.1359, "0.135")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Medium, 0.1219, "0.122")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Medium, 0.1222, "0.122")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Medium, 0.1239, "0.124")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Medium, 0.1269, "0.126")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Medium, 0.1319, "0.132")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Medium, 0.1359, "0.136")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.High, 0.1233, "0.123")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.High, 0.1235, "0.124")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.High, 0.1236, "0.124")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Low, 1.4231, "1.4")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Low, 1.4531, "1.5")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Low, 1.4731, "1.5")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Low, 1.5131, "1.5")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3124, "5.30")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3224, "5.30")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3284, "5.35")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3424, "5.35")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3724, "5.35")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3784, "5.40")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3824, "5.40")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3924, "5.40")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, 5.3124, "5.31")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, 5.3224, "5.32")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, 5.3284, "5.33")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, 5.3424, "5.34")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, 5.3724, "5.37")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, 5.3784, "5.38")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, 5.3824, "5.38")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, 5.3924, "5.39")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Low, 0, "")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Medium, double.NegativeInfinity, "")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.High, double.PositiveInfinity, "")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Low, -0, "")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, double.PositiveInfinity, "")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, double.NaN, "")]
    public void CorrectValueShouldBeReturned(MeasurementUnits units, MeasurementResolution resolution, double thickness, string expectedThickness)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units, resolution);
        
        // Act
        var displayedThickness = sut.GetDisplayedThickness(thickness, false);

        // Assert
        displayedThickness.ShouldBe(expectedThickness);
    }

    [Test]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 12190u, 200u, "0.120")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 12220u, 200u, "0.120")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 12370u, 200u, "0.125")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 126800u, 20u, "0.125")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 131900u, 20u, "0.130")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 135900u, 20u, "0.135")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Medium, 1219u, 2000u, "0.122")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Medium, 1222u, 2000u, "0.122")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Medium, 1239u, 2000u, "0.124")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Medium, 1269u, 2000u, "0.126")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Medium, 1319u, 2000u, "0.132")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Medium, 1359u, 2000u, "0.136")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.High, 1233u, 2000u, "0.123")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.High, 12350u, 200u, "0.124")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.High, 123600u, 20u, "0.124")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Low, 5602u, 200u, "1.4")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Low, 5720u, 200u, "1.5")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Low, 5799u, 200u, "1.5")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Low, 5957u, 200u, "1.5")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Medium, 20914u, 200u, "5.30")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Medium, 20954u, 200u, "5.30")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Medium, 20977u, 200u, "5.35")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Medium, 21033u, 200u, "5.35")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Medium, 21151u, 200u, "5.35")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.High, 20914u, 200u, "5.31")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.High, 20954u, 200u, "5.32")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.High, 21151u, 200u, "5.37")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.High, 21174u, 200u, "5.38")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.High, 21190u, 200u, "5.38")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.High, 21229u, 200u, "5.39")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 0u, 5430u, "")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Low, 0u, 2054u, "")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 30962u, 200u, "0.120")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 31038u, 200u, "0.120")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 31419u, 200u, "0.125")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 322070u, 20u, "0.125")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 335020u, 20u, "0.130")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 345180u, 20u, "0.135")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Medium, 3096u, 2000u, "0.122")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Medium, 3103u, 2000u, "0.122")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Medium, 3147u, 2000u, "0.124")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Medium, 3223u, 2000u, "0.126")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Medium, 3350u, 2000u, "0.132")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Medium, 3451u, 2000u, "0.136")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.High, 3131u, 2000u, "0.123")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.High, 31369u, 200u, "0.124")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.High, 31394u, 200u, "0.124")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Low, 14231u, 200u, "1.4")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Low, 14531u, 200u, "1.5")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Low, 14731u, 200u, "1.5")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Low, 15131u, 200u, "1.5")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Medium, 53124u, 200u, "5.30")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Medium, 53224u, 200u, "5.30")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Medium, 53284u, 200u, "5.35")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Medium, 53424u, 200u, "5.35")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Medium, 53724u, 200u, "5.35")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.High, 53124u, 200u, "5.31")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.High, 53224u, 200u, "5.32")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.High, 53724u, 200u, "5.37")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.High, 53784u, 200u, "5.38")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.High, 53824u, 200u, "5.38")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.High, 53924u, 200u, "5.39")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 0u, 7654u, "")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Low, 0u, 6540u, "")]
    public void WithDifferingMeasurementUnits_CorrectValueShouldBeReturned(MeasurementUnits measurementUnits, MeasurementUnits units, MeasurementResolution resolution, uint thickness, uint velocity, string expectedThickness)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units, resolution);
        
        // Act
        var displayedThickness = sut.GetDisplayedThickness(thickness, velocity, measurementUnits, false);

        // Assert
        displayedThickness.ShouldBe(expectedThickness);
    }

    [Test]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 0.12190, "0.120")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 0.12220, "0.120")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 0.12370, "0.125")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 0.126800, "0.125")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 0.131900, "0.130")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Low, 0.135900, "0.135")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Medium, 0.1219, "0.122")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.Medium, 0.1222, "0.122")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.High, 0.1233, "0.123")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.High, 0.12350, "0.124")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, MeasurementResolution.High, 0.123600, "0.124")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Low, 0.05602, "1.4")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Low, 0.05720, "1.5")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Medium, 0.20914, "5.30")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Medium, 0.20954, "5.30")]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, MeasurementResolution.Medium, 0.20977, "5.35")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 3.0962, "0.120")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 3.1038, "0.120")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 3.1419, "0.125")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 3.22070, "0.125")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Medium, 3.096, "0.122")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Medium, 3.223, "0.126")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Medium, 3.350, "0.132")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.High, 3.1369, "0.124")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.High, 3.1394, "0.124")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Low, 1.4231, "1.4")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Low, 1.4731, "1.5")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Low, 1.5131, "1.5")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3124, "5.30")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3224, "5.30")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Medium, 5.3724, "5.35")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.High, 5.3124, "5.31")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.High, 5.3224, "5.32")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.High, 5.3924, "5.39")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, MeasurementResolution.Low, 0.0, "")]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, MeasurementResolution.Low, 0.0, "")]
    public void WithMeasurementThicknessAndMeasurementUnits_CorrectValueShouldBeReturned(MeasurementUnits measurementUnits, MeasurementUnits units, MeasurementResolution resolution, double thickness, string expectedThickness)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units, resolution);
        
        // Act
        var displayedThickness = sut.GetDisplayedThickness(thickness, measurementUnits, false);

        // Assert
        displayedThickness.ShouldBe(expectedThickness);
    }
}
