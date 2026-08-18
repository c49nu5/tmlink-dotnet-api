using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenDeviceDisconnectedIsCalled
{

    [Test]
    public async Task AndGaugeIsConnected_ShouldSetIsConnectedToFalse()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.Disconnect()).Returns(Task.CompletedTask);
        testBed.ConnectionService.Setup(p => p.GaugeIsDisconnected(sut.DeviceIdentifier));

        // Act
        sut.DeviceDisconnected(sut.DeviceIdentifier);

        // Assert
        sut.IsConnected.ShouldBeFalse();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldInformTheConnectionService()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.Disconnect()).Returns(Task.CompletedTask);
        testBed.ConnectionService.Setup(p => p.GaugeIsDisconnected(sut.DeviceIdentifier));

        // Act
        sut.DeviceDisconnected(sut.DeviceIdentifier);

        // Assert
        testBed.ConnectionService.Verify(p => p.GaugeIsDisconnected(sut.DeviceIdentifier), Times.Once);
    }
}
