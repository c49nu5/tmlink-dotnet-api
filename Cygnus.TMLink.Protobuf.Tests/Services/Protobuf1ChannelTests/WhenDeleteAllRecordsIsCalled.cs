using Cygnus.TMLink.Protobuf.Interfaces;
using Cygnus.TMLink.Protobuf.Services;
using Moq;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenDeleteAllRecordsIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithDeleteAllRecordsCommandType()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllRecords), false)).ReturnsAsync(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllBScans), false)).ReturnsAsync(true);

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
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllRecords), false)).ReturnsAsync(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllBScans), false)).ReturnsAsync(true);

        // Act
        await sut.DeleteAllRecords();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllBScans), false), Times.Once);
    }
}
