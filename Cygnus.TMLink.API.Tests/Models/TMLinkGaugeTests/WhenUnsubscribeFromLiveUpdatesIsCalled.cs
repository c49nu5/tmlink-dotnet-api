using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenUnsubscribeFromLiveUpdatesIsCalled
{
    [Test]
    public void AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = () => sut.UnsubscribeFromLiveUpdates();

        // Assert
        act.ShouldThrow<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallUnsubscribeFromLiveUpdatesOnProtobufChannel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.UnsubscribeFromLiveUpdates());

        // Act
        sut.UnsubscribeFromLiveUpdates();

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.UnsubscribeFromLiveUpdates(), Times.Once);
    }
}
