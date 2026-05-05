using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetDisplayedThicknessFromMillimetresIsCalled
{
    [Test]
    [TestCase("10.1mm", 10.14343, MeasurementUnits.Metric, MeasurementResolution.Low)]
    [TestCase("0.1mm", 0.119342, MeasurementUnits.Metric, MeasurementResolution.Low)]
    [TestCase("10.15mm", 10.14324, MeasurementUnits.Metric, MeasurementResolution.Medium)]
    [TestCase("0.10mm", 0.11943, MeasurementUnits.Metric, MeasurementResolution.Medium)]
    [TestCase("10.14mm", 10.143243, MeasurementUnits.Metric, MeasurementResolution.High)]
    [TestCase("0.12mm", 0.119323, MeasurementUnits.Metric, MeasurementResolution.High)]
    [TestCase("10.045in" , 255.1324, MeasurementUnits.Imperial, MeasurementResolution.Low)]
    [TestCase("0.065in", 1.652432, MeasurementUnits.Imperial, MeasurementResolution.Low)]
    [TestCase("10.046in", 255.14324, MeasurementUnits.Imperial, MeasurementResolution.Medium)]
    [TestCase("0.066in", 1.652342, MeasurementUnits.Imperial, MeasurementResolution.Medium)]
    [TestCase("10.044in", 255.12342, MeasurementUnits.Imperial, MeasurementResolution.High)]
    [TestCase("0.065in", 1.65234, MeasurementUnits.Imperial, MeasurementResolution.High)]
    [TestCase("", 0, MeasurementUnits.Metric, MeasurementResolution.High)]
    [TestCase("" , 0, MeasurementUnits.Imperial, MeasurementResolution.Low)]
    public void CorrectValueShouldBeReturned(string displayedThickness, double mm, MeasurementUnits units, MeasurementResolution resolution)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units, resolution);

        // Act
        var result = sut.GetDisplayedThicknessFromMillimetres(mm);

        // Assert
        result.ShouldBe(displayedThickness);
    }
}
