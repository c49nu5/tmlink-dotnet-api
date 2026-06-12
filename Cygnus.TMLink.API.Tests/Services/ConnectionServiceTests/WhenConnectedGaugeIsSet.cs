using Cygnus.Interfaces;
using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Services.ConnectionServiceTests;
internal class WhenConnectedGaugeIsSet
{
    [Test]
    public void ShouldUpdateConnectedGaugeProperty()
    {
        // Arrange
        var testBed = new TestBed();
        var gauge = Mock.Of<IGauge>();
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
        var gauge = Mock.Of<IGauge>();
        testBed.Observer.Setup(m => m.GaugeConnected(gauge));
        testBed.Observer.SetupSet(o => o.ConnectionState = It.IsAny<ConnectionState>());

        // Act
        sut.ConnectedGauge = gauge;

        // Assert
        testBed.Observer.Verify(m => m.GaugeConnected(gauge), Times.Once);
    }

    [Test]
    public void ShouldNotifyObserversOfConnectionStateChange()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(true);
        var gauge = Mock.Of<IGauge>();
        testBed.Observer.Setup(m => m.GaugeConnected(gauge));
        testBed.Observer.SetupSet(o => o.ConnectionState = It.IsAny<ConnectionState>());

        // Act
        sut.ConnectedGauge = gauge;

        // Assert
        testBed.Observer.VerifySet(o => o.ConnectionState = ConnectionState.Connected, Times.Once);
    }
}
