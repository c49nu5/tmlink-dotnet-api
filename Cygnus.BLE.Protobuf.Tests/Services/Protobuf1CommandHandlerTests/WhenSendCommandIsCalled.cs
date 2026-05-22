using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Cygnus.BLE.Protobuf.V1;
using Moq;
using Shouldly;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1CommandHandlerTests;
internal class WhenSendCommandIsCalled
{
    [Test]
    public async Task ShouldSendTheExpectedBytesToTheWriteCharacteristic()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.ConfigureCommand(CommandType.DeleteAllRecords);

        // Act
        await sut.SendCommand(new Command { commandType = CommandType.DeleteAllRecords }, false);

        // Assert
        testBed.WriteCommandCharacteristic.Verify(c => c.WriteValueWithResponse(testBed.CommandBytes), Times.Once);
    }

    [Test]
    public async Task ShouldReturnTrueIfCommandIsSentSuccessfully()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.ConfigureCommand(CommandType.DeleteAllRecords);

        // Act
        var result = await sut.SendCommand(new Command { commandType = CommandType.DeleteAllRecords }, false);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public async Task ShouldReturnFalseIfUnexpectedNotificationIsReceived()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.ConfigureCommand(CommandType.DeleteAllRecords, 0);
        testBed.SendDelayedNotification(CommandType.GetGaugeInfo, 100);

        // Act
        var result = await sut.SendCommand(new Command { commandType = CommandType.DeleteAllRecords }, false);

        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public async Task ShouldNotThrowIfTaskIsCancelled()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.ConfigureCommand(CommandType.DeleteAllRecords, 0);
        var t = Task.Delay(100).ContinueWith(
            task =>
            {
                sut.CancelCommand();
            });

        // Act
        await sut.SendCommand(new Command { commandType = CommandType.DeleteAllRecords }, false);

        // Assert
        Assert.Pass();
    }
}
