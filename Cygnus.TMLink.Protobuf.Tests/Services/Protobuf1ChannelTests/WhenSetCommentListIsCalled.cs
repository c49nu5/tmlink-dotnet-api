using Cygnus.TMLink.Protobuf.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenSetCommentListIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithSetCommentListCommandType([Random(1, 7, 1)] int commentCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.SetCommentList), false)).ReturnsAsync(true);
        var commentsList = CreateCommentList(commentCount);

        // Act
        await sut.SendCommentList([..commentsList]);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.SetCommentList), false), Times.Once);
    }

    [Test]
    public async Task ShouldSendACommandWithExpectedComments([Random(1, 7, 1)] int commentCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.SetCommentList), false)).ReturnsAsync(true);
        var commentsList = CreateCommentList(commentCount);

        // Act
        await sut.SendCommentList([..commentsList]);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<V1.Command>(m => MatchingCommand(m, commentsList)), false), Times.Once);
    }

    private bool MatchingCommand(V1.Command m, List<string> commentsList)
    {
        m.commentList.Comments.ShouldBeEquivalentTo(commentsList); 
        return true;
    }

    private List<string> CreateCommentList(int commentCount)
    {
        return [.. Enumerable.Range(0, commentCount).Select(i => $"Com {i}")];
    }
}
