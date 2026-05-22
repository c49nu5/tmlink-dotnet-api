using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.V1;
using Moq;
using Shouldly;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1CommandHandlerTests;
internal class WhenDisconnectIsCalled
{
    [Test]
    public async Task ShouldRemoveTheEventHandlerFromTheNotifyCharacteristic()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.NotifyMessageCharacteristic.SetupRemove(c => c.CharacteristicValueChanged -= null);

        // Act
        sut.Disconnect();

        // Assert
        testBed.NotifyMessageCharacteristic.VerifyRemove(c => c.CharacteristicValueChanged -= It.Is<EventHandler<BLECharacteristicValueChangedEventArgs>>(a => a != null), Times.Once);
    }

    [Test]
    public async Task ThenSendCommandWithResponse_ShouldReturnNull()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.ConfigureCommand(CommandType.GetRecord, 500);
        testBed.ReadMessageCharacteristic.Setup(c => c.ReadValue()).ReturnsAsync(testBed.ReadBytes);
        Message.Record expectedValue = new() { recordType = RecordType.Linear };
        testBed.ProtobufMessageConverter.Setup(c => c.FromZippedProtoBuf<Message>(testBed.ReadBytes)).Returns(new Message { commandType = CommandType.GetRecord, record = expectedValue });

        // Act
        var t = Task.Delay(200).ContinueWith(
            task =>
            {
                sut.Disconnect();
            });
        var result = await sut.SendCommandWithResponse<Message.Record, Message>(new Command { commandType = CommandType.GetRecord }, m => m.record);

        // Assert
        result.ShouldBe(null);
    }
}

