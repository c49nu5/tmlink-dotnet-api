using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1CommandHandlerTests;
internal class TestBed
{
    public ILogger<Protobuf1CommandHandler> Logger { get; set; } = Mock.Of<ILogger<Protobuf1CommandHandler>>();
    public Mock<IProtobufMessageConverter> ProtobufMessageConverter { get; set; } = new Mock<IProtobufMessageConverter>(MockBehavior.Strict);
 
    public Mock<IBLECharacteristic> WriteCommandCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> NotifyMessageCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> ReadMessageCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> FrozenCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);

    public byte[] CommandBytes = [0x01, 0x02, 0x03, 0x04];
    public byte[] ReadBytes = [0x05, 0x06, 0x07];
    public byte[] NotifyBytes = [0x08, 0x09];

    internal Protobuf1CommandHandler CreateSUT()
    {
        FrozenCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkFrozenCharacteristicId);
        WriteCommandCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkWriteCommandCharacteristicId);
        ReadMessageCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkReadMessageCharacteristicId);
        NotifyMessageCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkNotifyMessageCharacteristicId);
        NotifyMessageCharacteristic.SetupAdd(c => c.CharacteristicValueChanged += null);
        NotifyMessageCharacteristic.Setup(c => c.StartNotifications()).Returns(Task.CompletedTask);
        return new Protobuf1CommandHandler(Logger, ProtobufMessageConverter?.Object);
    }

    internal async Task<Protobuf1CommandHandler> CreateConnectedSUT()
    {
        var sut = CreateSUT();

        await sut.Connect([
            WriteCommandCharacteristic.Object,
            NotifyMessageCharacteristic.Object,
            ReadMessageCharacteristic.Object,
            FrozenCharacteristic.Object
        ]);
        return sut;
    }

    internal void ConfigureCommand(V1.CommandType commandType, int notificationTimeSpan = 100)
    {
        WriteCommandCharacteristic.Setup(w => w.WriteValueWithResponse(CommandBytes)).Returns(Task.CompletedTask);
        ProtobufMessageConverter.Setup(c => c.ToZippedProtobuf(It.Is<ICommand>(m => m.CommandType == (CommandType)commandType))).Returns(CommandBytes);
        if (notificationTimeSpan > 0)
        {
            SendDelayedNotification(commandType, notificationTimeSpan);
        }
    }

    internal void SendDelayedNotification(V1.CommandType command, int notificationTimeSpan)
    {
        Task.Delay(notificationTimeSpan).ContinueWith(
            task =>
            {
                ProtobufMessageConverter.Setup(c => c.FromProtobuf<V1.NotifyMessage>(NotifyBytes)).Returns(new V1.NotifyMessage { commandType = command, readDataAvailable = true, errorCode = V1.ErrorCodes.Success });
                NotifyMessageCharacteristic.Raise(m => m.CharacteristicValueChanged -= null, null, new BLECharacteristicValueChangedEventArgs { Value = NotifyBytes });
            });
    }
}
