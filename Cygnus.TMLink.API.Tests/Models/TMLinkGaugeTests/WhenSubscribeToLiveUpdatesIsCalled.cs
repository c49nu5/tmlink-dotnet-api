using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenSubscribeToLiveUpdatesIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.SubscribeToLiveUpdates();

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallSubscribeToLiveUpdatesOnProtobufChannel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.SubscribeToLiveUpdates()).Returns(Task.CompletedTask);

        // Act
        await sut.SubscribeToLiveUpdates();

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.SubscribeToLiveUpdates(), Times.Once);
    }
}
