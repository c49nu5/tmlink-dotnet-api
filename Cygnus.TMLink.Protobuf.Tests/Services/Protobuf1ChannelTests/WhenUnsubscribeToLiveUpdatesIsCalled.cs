using Cygnus.TMLink.Interfaces;
using Moq;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenUnsubscribeFromLiveUpdatesIsCalled
{
    [Test]
    public async Task ShouldRemoveEventHandlerFromLiveUpdates()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.LiveCharacteristic.Setup(c => c.StopNotifications()).Returns(Task.CompletedTask);
        testBed.LiveCharacteristic.SetupRemove(c => c.CharacteristicValueChanged -= null);

        // Act
        await sut.UnsubscribeFromLiveUpdates();

        // Assert
        testBed.LiveCharacteristic.VerifyRemove(c => c.CharacteristicValueChanged -= It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Once);
    }

    [Test]
    public async Task AndChannelNotConnected_ShouldNotErrorAsync()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        await sut.UnsubscribeFromLiveUpdates();

        // Assert
        Assert.Pass();
    }
}
