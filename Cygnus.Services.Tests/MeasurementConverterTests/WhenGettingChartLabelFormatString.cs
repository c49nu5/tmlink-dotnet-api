using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;

internal class WhenGettingChartLabelFormatString
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, "0.0##")]
    [TestCase(MeasurementUnits.Metric, "0.##")]
    public void CorrectValueShouldBeReturned(MeasurementUnits units, string expectedFormatString)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units);
        
        // Act
        var formatString = sut.ChartLabelFormatString;

        // Assert
        formatString.ShouldBe(expectedFormatString);
    }
}
