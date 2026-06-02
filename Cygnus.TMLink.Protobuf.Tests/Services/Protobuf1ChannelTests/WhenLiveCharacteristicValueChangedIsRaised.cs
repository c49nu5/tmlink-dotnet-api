using Cygnus.TMLink.Interfaces;
using Cygnus.Models;
using Moq;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenLiveCharacteristicValueChangedIsRaised
{
    [Test]
    public async Task AndClientIsSubscribed_ShouldNotifyObserver([Values] bool isFrozen)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(configureForLiveUpdates: true, configureForFrozenUpdates: isFrozen);
        await sut.SubscribeToLiveUpdates();

        // Act
        testBed.LiveCharacteristic.Raise(s => s.CharacteristicValueChanged += null, null, new ValueChangedEventArgs { Value = testBed.LiveBytes });
        await Task.Delay(10); // Allow time for async event handler to execute

        // Assert
        testBed.Observer.Verify(o => o.OnLiveMeasurementReceived(It.IsAny<LiveMeasurement>()), Times.Once);
    }

    [Test]
    public async Task AndClientIsNotSubscribed_ShouldNotifyObserver()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(configureForLiveUpdates: true);

        // Act
        testBed.LiveCharacteristic.Raise(s => s.CharacteristicValueChanged += null, null, new ValueChangedEventArgs { Value = testBed.LiveBytes });

        // Assert
        testBed.Observer.Verify(o => o.OnLiveMeasurementReceived(It.IsAny<LiveMeasurement>()), Times.Never);
    }

    [Test]
    public async Task AndMeasurementIsFrozen_ShouldReadValueFromFrozenCharacteristic()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(configureForLiveUpdates: true, configureForFrozenUpdates: true);
        await sut.SubscribeToLiveUpdates();

        // Act
        testBed.LiveCharacteristic.Raise(s => s.CharacteristicValueChanged += null, null, new ValueChangedEventArgs { Value = testBed.LiveBytes });

        // Assert
        testBed.FrozenCharacteristic.Verify(c => c.ReadValue(), Times.Once);
    }
}
