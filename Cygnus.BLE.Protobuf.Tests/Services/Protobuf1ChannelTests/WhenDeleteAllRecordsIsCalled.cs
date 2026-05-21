using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Moq;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenDeleteAllRecordsIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithDeleteAllRecordsCommandType()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllRecords), false)).Returns(Task.CompletedTask);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllBScans), false)).Returns(Task.CompletedTask);

        // Act
        await sut.DeleteAllRecords();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllRecords), false), Times.Once);
    }

    [Test]
    public async Task ShouldSendACommandWithDeleteAllBScansCommandType()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllRecords), false)).Returns(Task.CompletedTask);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllBScans), false)).Returns(Task.CompletedTask);

        // Act
        await sut.DeleteAllRecords();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllBScans), false), Times.Once);
    }
}
