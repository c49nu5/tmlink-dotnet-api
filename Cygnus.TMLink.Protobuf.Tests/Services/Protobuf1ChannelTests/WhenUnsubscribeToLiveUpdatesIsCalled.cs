using Cygnus.Interfaces;
using Cygnus.TMLink.Interfaces;
using Moq;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenRemoveObserverIsCalled
{
    [Test]
    public async Task ShouldRemoveEventHandlerFromLiveUpdates()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(configureForLiveUpdates: true);
        testBed.LiveCharacteristic.Setup(c => c.StopNotifications()).Returns(Task.CompletedTask);
        testBed.LiveCharacteristic.SetupRemove(c => c.CharacteristicValueChanged -= null);

        // Act
        sut.RemoveObserver(testBed.Observer.Object);

        // Assert
        testBed.LiveCharacteristic.VerifyRemove(c => c.CharacteristicValueChanged -= It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Once);
    }

    [Test]
    public async Task AndObserversRemain_ShouldNotRemoveEventHandlerFromLiveUpdates()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(configureForLiveUpdates: true);
        testBed.LiveCharacteristic.Setup(c => c.StopNotifications()).Returns(Task.CompletedTask);
        testBed.LiveCharacteristic.SetupRemove(c => c.CharacteristicValueChanged -= null);
        sut.AddObserver(new Mock<ILiveMeasurementObserver>().Object);

        // Act
        sut.RemoveObserver(testBed.Observer.Object);

        // Assert
        testBed.LiveCharacteristic.VerifyRemove(c => c.CharacteristicValueChanged -= It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Never);
    }

    [Test]
    public async Task AndChannelNotConnected_ShouldNotErrorAsync()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        sut.RemoveObserver(testBed.Observer.Object);

        // Assert
        Assert.Pass();
    }
}
