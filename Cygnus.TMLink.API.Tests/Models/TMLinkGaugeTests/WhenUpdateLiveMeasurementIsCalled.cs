using Cygnus.Models;
using Moq;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenUpdateLiveMeasurementIsCalled
{
    [Test]
    public void ObserverShouldBeNotified()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(true);        
        LiveMeasurement liveMeasurement = new(){ IsFrozen = true };
        testBed.Observer.Setup(o => o.OnLiveMeasurementReceived(liveMeasurement));

        // Act
        sut.OnLiveMeasurementReceived(liveMeasurement);

        // Assert
        testBed.Observer.Verify(o => o.OnLiveMeasurementReceived(liveMeasurement), Times.Once);
    }
}
