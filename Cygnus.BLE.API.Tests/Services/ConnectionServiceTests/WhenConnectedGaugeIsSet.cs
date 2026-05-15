using Cygnus.BLE.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.BLE.API.Tests.Services.ConnectionServiceTests;
internal class WhenConnectedGaugeIsSet
{
    [Test]
    public void ShouldUpdateConnectedGaugeProperty()
    {
        // Arrange
        var testBed = new TestBed();
        var gauge = Mock.Of<IBLEGauge>();
        var sut = testBed.CreateSUT();

        // Act
        sut.ConnectedGauge = gauge;

        // Assert
        sut.ConnectedGauge.ShouldBe(gauge);
    }

    [Test]
    public void ShouldNotifyObserversOfGaugeConnectionChange()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(true);
        var gauge = Mock.Of<IBLEGauge>();
        testBed.Observer.Setup(m => m.GaugeConnected(gauge));

        // Act
        sut.ConnectedGauge = gauge;

        // Assert
        testBed.Observer.Verify(m => m.GaugeConnected(gauge), Times.Once);
    }
}
