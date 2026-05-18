using Cygnus.BLE.Protobuf.Interfaces;
using Moq;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenCancelRecordTransfer
{
    [Test]
    public async Task ShouldSendACommandWithCancelRecordTransferCommandType()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(CommandType.CancelRecordTransfer);
        byte[] commandBytes = [0x01, 0x02, 0x03, 0x04];
        testBed.WriteCommandCharacteristic.Setup(w => w.WriteValueWithResponse(commandBytes)).Returns(Task.CompletedTask);
        testBed.ProtobufMessageConverter.Setup(c => c.ToZippedProtobuf(It.Is<ICommand>(m => m.CommandType == CommandType.CancelRecordTransfer))).Returns(commandBytes);

        // Act
        await sut.CancelRecordTransfer();

        // Assert
        testBed.ProtobufMessageConverter.Verify(c => c.ToZippedProtobuf(It.Is<ICommand>(m => m.CommandType == CommandType.CancelRecordTransfer)), Times.Once);
    }
}
