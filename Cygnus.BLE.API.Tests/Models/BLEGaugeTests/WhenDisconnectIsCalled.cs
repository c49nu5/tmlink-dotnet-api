using Moq;
using Shouldly;

namespace Cygnus.BLE.API.Tests.Models.BLEGaugeTests;
internal class WhenDisconnectIsCalled
{
    [Test]
    public async Task AndGaugeIsConnected_ShouldCallDisconnectOnProtobufChannel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.Disconnect());
        testBed.ConnectionService.Setup(p => p.GaugeIsDisconnected(sut.DeviceIdentifier));

        // Act
        sut.Disconnect();

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.Disconnect(), Times.Once);
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldSetIsConnectedToFalse()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.Disconnect());
        testBed.ConnectionService.Setup(p => p.GaugeIsDisconnected(sut.DeviceIdentifier));

        // Act
        sut.Disconnect();

        // Assert
        sut.IsConnected.ShouldBeFalse();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldInformTheConnectionService()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.Disconnect());
        testBed.ConnectionService.Setup(p => p.GaugeIsDisconnected(sut.DeviceIdentifier));

        // Act
        sut.Disconnect();

        // Assert
        testBed.ConnectionService.Verify(p => p.GaugeIsDisconnected(sut.DeviceIdentifier), Times.Once);
    }
}
