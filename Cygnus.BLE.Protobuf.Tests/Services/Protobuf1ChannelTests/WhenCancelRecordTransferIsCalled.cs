using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Moq;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenCancelRecordTransferIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithCancelRecordTransferCommandType()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);

        // Act
        await sut.CancelRecordTransfer();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.CancelRecordTransfer), true), Times.Once);
    }
}
