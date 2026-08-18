using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenUnsubscribeFromLiveUpdatesIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldNotThrow()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.UnsubscribeFromLiveUpdates();

        // Assert
        await act.ShouldNotThrowAsync();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallUnsubscribeFromLiveUpdatesOnProtobufChannel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.UnsubscribeFromLiveUpdates()).Returns(Task.CompletedTask);

        // Act
        await sut.UnsubscribeFromLiveUpdates();

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.UnsubscribeFromLiveUpdates(), Times.Once);
    }
}
