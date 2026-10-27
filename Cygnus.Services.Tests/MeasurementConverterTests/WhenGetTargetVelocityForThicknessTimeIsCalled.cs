using Cygnus.Models;
using Shouldly;

namespace Cygnus.Services.Test.MeasurementConverterTests;
internal class WhenGetTargetVelocityForThicknessTimeIsCalled
{
    [Test]
    [TestCase(MeasurementUnits.Imperial, 15.1219, 2323u, 3543u)]
    [TestCase(MeasurementUnits.Imperial, 0.1319, 323u, 3543u)]
    [TestCase(MeasurementUnits.Imperial, 0.1219, 2323u, 1049u)]
    [TestCase(MeasurementUnits.Imperial, 0.1222, 3323u, 735u)]
    [TestCase(MeasurementUnits.Imperial, 0.1237, 4323u, 572u)]
    [TestCase(MeasurementUnits.Imperial, 0.1268, 5323u, 476u)]
    [TestCase(MeasurementUnits.Imperial, 0.1319, 6323u, 417u)]
    [TestCase(MeasurementUnits.Imperial, 0.1359, 7323u, 394u)]
    [TestCase(MeasurementUnits.Imperial, 0.1219, 8323u, 394u)]
    [TestCase(MeasurementUnits.Metric, 1.4231, 5323u, 1000u)]
    [TestCase(MeasurementUnits.Metric, 13.4231, 5323u, 5043u)]
    [TestCase(MeasurementUnits.Metric, 12.4531, 6323u, 3938u)]
    [TestCase(MeasurementUnits.Metric, 15.4731, 6523u, 4744u)]
    [TestCase(MeasurementUnits.Metric, 16.5131, 4323u, 7639u)]
    [TestCase(MeasurementUnits.Metric, 16.5131, 1323u, 9000u)]
    public void CorrectValueShouldBeReturned(MeasurementUnits units, double targetThickness, uint calibratedThickness, uint expectedVelocity)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(units);
        
        // Act
        var targetVelocity = sut.GetTargetVelocityForThicknessTime(targetThickness, calibratedThickness);

        // Assert
        targetVelocity.ShouldBe(expectedVelocity);
    }
}
