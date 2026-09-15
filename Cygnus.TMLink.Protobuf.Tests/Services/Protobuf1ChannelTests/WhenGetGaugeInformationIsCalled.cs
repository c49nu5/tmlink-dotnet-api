using Cygnus.TMLink.Protobuf.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenGetGaugeInformationIsCalled
{
    [Test]
    public async Task ShouldDelegateToTheCommandHandler()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        V1.Message.GaugeInfo protobufInfo = new();
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetGaugeInfo))).ReturnsAsync(new V1.Message { gaugeInfo = protobufInfo });

        // Act
        var gaugeInfo = await sut.GetGaugeInformation();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetGaugeInfo)), Times.Once());
    }

    [Test]
    public async Task ReturnReturnGaugeInfo_WithExpectedBatteryLevel([Random(0u, 100u, 1)] uint batteryLevel)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        V1.Message.GaugeInfo protobufInfo = new V1.Message.GaugeInfo { batteryLevel = batteryLevel };
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetGaugeInfo))).ReturnsAsync(new V1.Message { gaugeInfo = protobufInfo });

        // Act
        var gaugeInfo = await sut.GetGaugeInformation();

        // Assert
        gaugeInfo?.BatteryLevel.ShouldBe(batteryLevel);
    }

    [Test]
    public async Task ReturnReturnGaugeInfo_WithExpectedGaugeId([Random(0u, 100u, 1)] uint gaugeUD)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        V1.Message.GaugeInfo protobufInfo = new V1.Message.GaugeInfo { gaugeUD = gaugeUD };
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetGaugeInfo))).ReturnsAsync(new V1.Message { gaugeInfo = protobufInfo });

        // Act
        var gaugeInfo = await sut.GetGaugeInformation();

        // Assert
        gaugeInfo?.GaugeId.ShouldBe(gaugeUD);
    }

    [Test]
    public async Task ReturnReturnGaugeInfo_WithExpectedGaugeVariant([Values] V1.GaugeVariant gaugeVariant)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        V1.Message.GaugeInfo protobufInfo = new V1.Message.GaugeInfo { gaugeVariant = gaugeVariant };
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetGaugeInfo))).ReturnsAsync(new V1.Message { gaugeInfo = protobufInfo });

        // Act
        var gaugeInfo = await sut.GetGaugeInformation();

        // Assert
        ((int)gaugeInfo?.GaugeVariant.Value).ShouldBe((int)gaugeVariant);
    }

    [Test]
    public async Task ReturnReturnGaugeInfo_WithExpectedSerialNumber([Random(0u, UInt32.MaxValue, 1)] uint serialNumber)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        V1.Message.GaugeInfo protobufInfo = new V1.Message.GaugeInfo { serialNumber = serialNumber };
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetGaugeInfo))).ReturnsAsync(new V1.Message { gaugeInfo = protobufInfo });

        // Act
        var gaugeInfo = await sut.GetGaugeInformation();

        // Assert
        gaugeInfo?.SerialNumber.ShouldBe(serialNumber);
    }

    [Test]
    public async Task ReturnReturnGaugeInfo_WithExpectedVersionNumber([Random(0u, 100u, 1)] uint versionNumber)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        V1.Message.GaugeInfo protobufInfo = new V1.Message.GaugeInfo { versionNumber = versionNumber };
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetGaugeInfo))).ReturnsAsync(new V1.Message { gaugeInfo = protobufInfo });

        // Act
        var gaugeInfo = await sut.GetGaugeInformation();

        // Assert
        gaugeInfo?.SoftwareVersionNumber.ShouldBe(versionNumber);
    }
}
