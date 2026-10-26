using Cygnus.TMLink.Protobuf.Interfaces;
using Moq;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenDeleteAllTemplatesIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithDeleteAllTemplatesCommandType()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllTemplates), false)).ReturnsAsync(true);

        // Act
        await sut.DeleteAllTemplates();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.DeleteAllTemplates), false), Times.Once);
    }
}
