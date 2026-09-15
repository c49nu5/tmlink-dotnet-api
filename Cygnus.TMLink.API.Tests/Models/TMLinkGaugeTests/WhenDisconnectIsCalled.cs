using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenDisconnectIsCalled
{
    [Test]
    public async Task WhenGaugeIsNotConnected_ShouldCallDisconnectOnConnectedDevice()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(d => d.Disconnect()).Returns(Task.CompletedTask);

        // Act
        await sut.Disconnect();

        // Assert
        testBed.ConnectedDevice.Verify(d => d.Disconnect(), Times.Once);
    }

    [Test]
    public async Task WhenDeviceIsNull_IsConnectedShouldBeFalse()
    {
        // Arrange
        var tb = new TestBed();
        var sut = tb.CreateSUT();
        tb.Protobuf1Channel.Setup(p => p.Disconnect());
        tb.ConnectionService.Setup(s => s.GaugeIsDisconnected(sut.DeviceIdentifier));

        // Act
        await sut.Disconnect();

        // Assert
        sut.IsConnected.ShouldBeFalse();
    }

    [Test]
    public async Task WhenDeviceIsNull_NotifiesConnectionService()
    {
        // Arrange
        var tb = new TestBed();
        var sut = tb.CreateSUT();
        tb.Protobuf1Channel.Setup(p => p.Disconnect());
        tb.ConnectionService.Setup(s => s.GaugeIsDisconnected(sut.DeviceIdentifier));

        // Act
        await sut.Disconnect();

        // Assert
        tb.ConnectionService.Verify(s => s.GaugeIsDisconnected(sut.DeviceIdentifier), Times.Once);
    }

    [Test]
    public async Task WhenDeviceIsNotConnected_DoesNotCallDeviceDisconnect()
    {
        // Arrange
        var tb = new TestBed();
        var sut = tb.CreateSUT();
        var device = tb.CreateDevice();
        device.SetupGet(d => d.IsConnected).Returns(false);

        sut.SetDevice(device.Object);
        tb.Protobuf1Channel.Setup(p => p.Disconnect());
        tb.ConnectionService.Setup(s => s.GaugeIsDisconnected(sut.DeviceIdentifier));

        // Act
        await sut.Disconnect();

        // Assert
        device.Verify(d => d.Disconnect(), Times.Never);
    }

    [Test]
    public async Task WhenDeviceIsNotConnected_SetIsConnectedToFalse()
    {
        // Arrange
        var tb = new TestBed();
        var sut = tb.CreateSUT();
        var device = tb.CreateDevice();
        device.SetupGet(d => d.IsConnected).Returns(false);

        sut.SetDevice(device.Object);
        tb.Protobuf1Channel.Setup(p => p.Disconnect());
        tb.ConnectionService.Setup(s => s.GaugeIsDisconnected(sut.DeviceIdentifier));

        // Act
        await sut.Disconnect();

        // Assert
        sut.IsConnected.ShouldBeFalse();
    }
}
