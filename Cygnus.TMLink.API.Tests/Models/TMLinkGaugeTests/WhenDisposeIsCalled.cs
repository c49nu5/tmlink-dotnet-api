using Moq;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenDisposeIsCalled
{
    [Test]
    public async Task AndGaugeIsConnected_ShouldCallDisposeOnProtobufChannel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(true);
        var device = testBed.CreateDevice(true);
        device.Setup(d => d.IsConnected).Returns(true);
        sut.SetDevice(device.Object);
        testBed.Protobuf1Channel.Setup(p => p.AddObserver(sut));
        await sut.Connect();
        testBed.Protobuf1Channel.Setup(p => p.Dispose());
        device.Setup(d => d.Dispose());

        // Act
        sut.Dispose();

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.Dispose(), Times.Once);
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallDisposeOnDevice()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(true);
        var device = testBed.CreateDevice(true);
        device.Setup(d => d.IsConnected).Returns(true);
        sut.SetDevice(device.Object);
        await sut.Connect();
        testBed.Protobuf1Channel.Setup(p => p.Dispose());
        device.Setup(d => d.Dispose());

        // Act
        sut.Dispose();

        // Assert
        device.Verify(d => d.Dispose(), Times.Once);
    }
}
