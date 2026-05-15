using Cygnus.Models;
using Moq;

namespace Cygnus.BLE.API.Tests.Models.BLEGaugeTests;
internal class WhenUpdateLiveMeasurementIsCalled
{
    [Test]
    public void ObserverShouldBeNotified()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(true);        
        LiveMeasurement liveMeasurement = new();
        testBed.Observer.Setup(o => o.UpdateLiveMeasurement(liveMeasurement));

        // Act
        sut.UpdateLiveMeasurement(liveMeasurement);

        // Assert
        testBed.Observer.Verify(o => o.UpdateLiveMeasurement(liveMeasurement), Times.Once);
    }
}
