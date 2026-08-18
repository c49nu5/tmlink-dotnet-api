using Cygnus.TMLink.Interfaces;
using Cygnus.TMLink.Protobuf.Interfaces;
using Moq;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
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
        await sut.Disconnect();

        // Assert
        testBed.LiveCharacteristic.VerifyRemove(c => c.CharacteristicValueChanged -= It.IsAny<EventHandler<ValueChangedEventArgs>>(), Times.Once);
    }

    [Test]
    public async Task ShouldCancelRecordTransfer()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.PrepareForDisconnect();

        // Act
        await sut.Disconnect();

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
        await sut.Disconnect();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.Disconnect(), Times.Once);
    }
}
