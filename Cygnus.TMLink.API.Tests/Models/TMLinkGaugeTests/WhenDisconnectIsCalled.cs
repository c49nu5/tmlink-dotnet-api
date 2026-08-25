using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenDisconnectIsCalled
{

    [Test]
    public async Task AndGaugeIsNotConnected_ShouldCallDisconnectOnConnectedDevice()
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
}
