using Cygnus.TMLink.API.Interfaces;
using Cygnus.TMLink.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Services.ConnectionServiceTests;
internal class WhenConnectToGaugeIsCalled
{
    [Test]
    public async Task ShouldCancelDiscovery()
    {
        // Arrange
        var testBed = new TestBed();
        testBed.DeviceDiscoverer.Setup(g => g.Cancel());
        var gauge = Mock.Of<ITMLinkGauge>();
        var sut = testBed.CreateSUT();

        // Act
        await sut.ConnectToGauge(gauge);

        // Assert
        testBed.DeviceDiscoverer.Verify(g => g.Cancel(), Times.Once);
    }

    [Test]
    public async Task AndGaugeIsNotConnected_ShouldCallConnectOnGauge()
    {
        // Arrange
        var testBed = new TestBed();
        testBed.DeviceDiscoverer.Setup(g => g.Cancel());
        var gauge = new Mock<ITMLinkGaugeInternal>();
        gauge.Setup(g => g.Connect()).ReturnsAsync(true);
        var sut = testBed.CreateSUT();

        // Act
        await sut.ConnectToGauge(gauge.Object);

        // Assert
        gauge.Verify(g => g.Connect(), Times.Once);
    }

    [Test]
    public async Task AndGaugeConnects_ShouldUpdateConnectedGaugeProperty()
    {
        // Arrange
        var testBed = new TestBed();
        testBed.DeviceDiscoverer.Setup(g => g.Cancel());
        var gauge = new Mock<ITMLinkGaugeInternal>();
        gauge.Setup(g => g.Connect()).ReturnsAsync(true);
        var sut = testBed.CreateSUT();

        // Act
        await sut.ConnectToGauge(gauge.Object);

        // Assert
        sut.ConnectedGauge.ShouldBe(gauge.Object);
    }

    [Test]
    public async Task AndGaugeDoesNotConnect_ShouldNotUpdateConnectedGaugeProperty()
    {
        // Arrange
        var testBed = new TestBed();
        testBed.DeviceDiscoverer.Setup(g => g.Cancel());
        var gauge = new Mock<ITMLinkGaugeInternal>();
        gauge.Setup(g => g.Connect()).ReturnsAsync(false);
        var sut = testBed.CreateSUT();

        // Act
        await sut.ConnectToGauge(gauge.Object);

        // Assert
        sut.ConnectedGauge.ShouldBeNull();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldUpdateConnectedGaugeProperty()
    {
        // Arrange
        var testBed = new TestBed();
        var gauge = Mock.Of<ITMLinkGaugeInternal>(g => g.IsConnected == true);
        testBed.DeviceDiscoverer.Setup(g => g.Cancel());
        var sut = testBed.CreateSUT();

        // Act
        await sut.ConnectToGauge(gauge);

        // Assert
        sut.ConnectedGauge.ShouldBe(gauge);
    }
}
