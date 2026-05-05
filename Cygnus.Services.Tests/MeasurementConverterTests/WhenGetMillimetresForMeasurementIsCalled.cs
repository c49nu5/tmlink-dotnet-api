using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetMillimetresForMeasurementIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 1219u, 2000u, 3.09626d)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 1319u, 2000u, 3.35026d)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 1359u, 2000u, 3.4518600000000004d)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 1423u, 2000u, 1.423)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 1513u, 2000u, 1.5130000000000001d)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 5312u, 2000u, 5.312)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Imperial, 0u, 2000u, 0)]
    [TestCase(MeasurementUnits.Imperial, MeasurementUnits.Metric, 0u, 2000u, 0)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 1219u, 2000u, 3.09626d)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 1319u, 2000u, 3.35026d)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 1359u, 2000u, 3.4518600000000004d)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 1423u, 2000u, 1.423)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 1513u, 2000u, 1.5130000000000001d)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 5312u, 2000u, 5.312)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Imperial, 0u, 2000u, 0)]
    [TestCase(MeasurementUnits.Metric, MeasurementUnits.Metric, 0u, 2000u, 0)]
    public void WithDifferingTimesAndVelocities_CorrectValueShouldBeReturned_RegardlessOfTheSettingsUnits(MeasurementUnits settingsUnits, MeasurementUnits units, uint thickness, uint velocity, double expectedMillimetres)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(settingsUnits);
        
        // Act
        var millimetres = sut.GetMillimetresForMeasurement(units, thickness, velocity);

        // Assert
        millimetres.ShouldBe(expectedMillimetres);
    }
}
