using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Moq;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenUnsubscribeFromLiveUpdatesIsCalled
{
    [Test]
    public async Task ShouldRemoveEventHandlerFromLiveUpdates()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.LiveCharacteristic.SetupRemove(c => c.CharacteristicValueChanged -= null);

        // Act
        sut.UnsubscribeFromLiveUpdates();

        // Assert
        testBed.LiveCharacteristic.VerifyRemove(c => c.CharacteristicValueChanged -= It.IsAny<EventHandler<BLECharacteristicValueChangedEventArgs>>(), Times.Once);
    }

    [Test]
    public void AndChannelNotConnected_ShouldNotError()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        sut.UnsubscribeFromLiveUpdates();

        // Assert
        Assert.Pass();
    }
}
