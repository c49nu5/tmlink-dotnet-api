using Cygnus.TMLink.Interfaces;
using Moq;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenSubscribeToLiveUpdatesIsCalled
{
    [Test]
    public async Task ShouldAttachEventHandlerToLiveUpdates()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.LiveCharacteristic.SetupAdd(c => c.CharacteristicValueChanged += null);
        testBed.LiveCharacteristic.Setup(c => c.StartNotifications()).Returns(Task.CompletedTask);

        // Act
        await sut.SubscribeToLiveUpdates();

        // Assert
        testBed.LiveCharacteristic.VerifyAdd(c => c.CharacteristicValueChanged += It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Once);
    }

    [Test]
    public async Task ShouldCallStartNotificationsOnLiveCharacteristic()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.LiveCharacteristic.SetupAdd(c => c.CharacteristicValueChanged += null);
        testBed.LiveCharacteristic.Setup(c => c.StartNotifications()).Returns(Task.CompletedTask);

        // Act
        await sut.SubscribeToLiveUpdates();

        // Assert
        testBed.LiveCharacteristic.Verify(c => c.StartNotifications(), Times.Once);
    }

    [Test]
    public async Task AndChannelNotConnected_ShouldNotErrorAsync()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        await sut.SubscribeToLiveUpdates();

        // Assert
        Assert.Pass();
    }
}
