using Cygnus.BLE.API.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.BLE.API.Tests.Services.ConnectionServiceTests;
internal class WhenGaugeIsDisconnectedIsCalled
{
    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("TestGauge")]
    public void AndNoGaugeIsConnected_ShouldNotUpdateConnectedGaugeProperty(string gaugeIdentifier)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        sut.GaugeIsDisconnected(gaugeIdentifier);

        // Assert
        sut.ConnectedGauge.ShouldBeNull();
    }

    [Test]
    public void AndItIsTheConnectedGauge_ShouldUpdateConnectedGaugeProperty()
    {
        // Arrange
        var testBed = new TestBed();
        var gauge = Mock.Of<IBLEGauge>(g=> g.DeviceIdentifier == "TestGauge");        
        var sut = testBed.CreateSUT();
        sut.ConnectedGauge = gauge;

        // Act
        sut.GaugeIsDisconnected("TestGauge");

        // Assert
        sut.ConnectedGauge.ShouldBeNull();
    }

    [Test]
    public void AndItIsNotTheConnectedGauge_ShouldNotUpdateConnectedGaugeProperty()
    {
        // Arrange
        var testBed = new TestBed();
        var gauge = Mock.Of<IBLEGauge>(g => g.DeviceIdentifier == "TestGauge" && g.IsConnected == true);
        var sut = testBed.CreateSUT();
        sut.ConnectedGauge = gauge;

        // Act
        sut.GaugeIsDisconnected("OtherTestGauge");

        // Assert
        sut.ConnectedGauge.ShouldBe(gauge);
    }
}
