using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

internal class WhenSendCommentListIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException([Random(1,7,1)] int commentCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.SendCommentList(CreateCommentList(commentCount));

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallSendCommentListOnProtobufChannelWithExpectedParameters([Random(1, 7, 1)] int commentCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.SendCommentList(It.IsAny<string[]>())).Returns(Task.CompletedTask);
        string[] commentsList = CreateCommentList(commentCount);

        // Act
        await sut.SendCommentList(commentsList);

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.SendCommentList(commentsList), Times.Once);
    }

    private string[] CreateCommentList(int commentCount)
    {
        return [.. Enumerable.Range(0, commentCount).Select(i => $"Com {i}")];
    }
}
