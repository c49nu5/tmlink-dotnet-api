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
        await sut.Connect(testBed.Device.Object);

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
        await sut.Connect(testBed.Device.Object);

        // Assert
        testBed.ProtobufCommandHandler.Verify(h => h.Connect(testBed.Characteristics), Times.Once);
    }

    [Test]
    public async Task AndConnectSuceeds_ShouldReturnExpectedGaugeInformation()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        var expectedInfo = testBed.PrepareForConnect();

        // Act
        var gaugeInfo = await sut.Connect(testBed.Device.Object);

        // Assert
        gaugeInfo?.BatteryLevel.ShouldBe(expectedInfo.batteryLevel);
        gaugeInfo?.SerialNumber.ShouldBe(expectedInfo.serialNumber);
        gaugeInfo?.SoftwareVersionNumber.ShouldBe(expectedInfo.versionNumber);
    }

    [Test]
    public async Task AndConnectToProtobufCommandHandlerFails_ShouldReturnNull()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        testBed.Device.Setup(d => d.GetCharacteristics(Constants.TMLinkServiceId)).ReturnsAsync(testBed.Characteristics);
        testBed.ProtobufCommandHandler.Setup(h => h.Connect(testBed.Characteristics)).ReturnsAsync(false);

        // Act
        var gaugeInfo = await sut.Connect(testBed.Device.Object);

        // Assert
        gaugeInfo.ShouldBeNull();
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
        var gaugeInfo = await sut.Connect(testBed.Device.Object);

        // Assert
        gaugeInfo.ShouldBeNull();
    }
}
