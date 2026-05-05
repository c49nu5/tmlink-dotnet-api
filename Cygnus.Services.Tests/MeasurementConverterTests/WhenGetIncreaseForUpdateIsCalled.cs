using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetIncreaseForUpdateIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Low, .1)]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Medium, .05)]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.High, 0.01)]
    [TestCase(MeasurementUnits.Metric, MeasurementResolution.Default, .05)]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Low, .005)]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Medium, 0.002)]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.High, .001)]
    [TestCase(MeasurementUnits.Imperial, MeasurementResolution.Default, 0.002)]

    public void CorrectValueShouldBeReturned(MeasurementUnits units, MeasurementResolution resolution, double expected)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        testBed.MeasurementSettingsService.Setup(m => m.Units).Returns(units);
        testBed.MeasurementSettingsService.Setup(m => m.Resolution).Returns(resolution);

        // Act
        var result = sut.GetThicknessIncrement();

        // Assert
        result.ShouldBe(expected);
    }
}
