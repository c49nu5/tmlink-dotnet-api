using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGettingThicknessFormatString
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Low, "F3")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Medium, "F3")]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.High, "F3")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Low, "F1")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, "F2")]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, "F2")]
    public void CorrectValueShouldBeReturned(MeasurementUnits units, MeasurementResolution resolution, string expectedFormatString)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units, resolution);
        
        // Act
        var formatString = sut.ThicknessFormatString;

        // Assert
        formatString.ShouldBe(expectedFormatString);
    }
}
