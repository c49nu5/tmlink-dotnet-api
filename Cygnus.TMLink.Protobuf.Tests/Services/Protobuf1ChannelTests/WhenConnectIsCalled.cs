using Cygnus.TMLink.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenConnectIsCalled
{
    [Test]
    public async Task ShouldGetCharacteristicsForTheTMLinkServiceFromDevice()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        testBed.PrepareForConnect();

        // Act
        await sut.Connect(testBed.Device.Object, testBed.Gauge.Object);

        // Assert
        testBed.Device.Verify(d => d.GetCharacteristics(Constants.TMLinkServiceId), Times.Once);
    }

    [Test]
    public async Task ShouldPassTheDeviceCharacteristicsToTheProtobufCommandHandler()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        testBed.PrepareForConnect();

        // Act
        await sut.Connect(testBed.Device.Object, testBed.Gauge.Object);

        // Assert
        testBed.ProtobufCommandHandler.Verify(h => h.Connect(testBed.Characteristics), Times.Once);
    }

    [Test]
    public async Task AndConnectSuceeds_ShouldReturnTrue()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        var expectedInfo = testBed.PrepareForConnect();

        // Act
        var result = await sut.Connect(testBed.Device.Object, testBed.Gauge.Object);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public async Task AndConnectToProtobufCommandHandlerFails_ShouldReturnFalse()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        testBed.Device.Setup(d => d.GetCharacteristics(Constants.TMLinkServiceId)).ReturnsAsync(testBed.Characteristics);
        testBed.ProtobufCommandHandler.Setup(h => h.Connect(testBed.Characteristics)).ReturnsAsync(false);

        // Act
        var result = await sut.Connect(testBed.Device.Object, testBed.Gauge.Object);

        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public async Task AndDeviceDoesNotReturnCharacteristics_ShouldReturnNull()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        testBed.Device.SetupGet(g => g.Name).Returns("Test Gauge");
        testBed.Device.Setup(d => d.GetCharacteristics(Constants.TMLinkServiceId)).ReturnsAsync((ITMLinkCharacteristic[])null);

        // Act
        var result = await sut.Connect(testBed.Device.Object, testBed.Gauge.Object);

        // Assert
        result.ShouldBeFalse();
    }
}
