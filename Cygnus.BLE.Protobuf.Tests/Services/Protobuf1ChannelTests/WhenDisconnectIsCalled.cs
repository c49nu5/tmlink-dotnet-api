using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Moq;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenDisconnectIsCalled
{
    [Test]
    public async Task ShouldUnsubscribeFromLiveUpdates()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.PrepareForDisconnect();

        // Act
        sut.Disconnect();

        // Assert
        testBed.LiveCharacteristic.VerifyRemove(c => c.CharacteristicValueChanged -= It.IsAny<EventHandler<BLECharacteristicValueChangedEventArgs>>(), Times.Once);
    }

    [Test]
    public async Task ShouldCancelRecordTransfer()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.PrepareForDisconnect();

        // Act
        sut.Disconnect();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.CancelRecordTransfer), true), Times.Once);
    }

    [Test]
    public async Task ShouldDisconnectTheProtobufCommandHandler()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.PrepareForDisconnect();

        // Act
        sut.Disconnect();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.Disconnect(), Times.Once);
    }
}
