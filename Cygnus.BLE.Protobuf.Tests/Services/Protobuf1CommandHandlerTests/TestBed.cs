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
    public Mock<IBLECharacteristic> NotifyMessageCharacteristic { get; set; } = new Mock<IBLECharacteristic>();
    public Mock<IBLECharacteristic> ReadMessageCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
   
    public byte[] CommandBytes = [0x01, 0x02, 0x03, 0x04];
    public byte[] ReadBytes = [0x05, 0x06, 0x07];
    public byte[] NotifyBytes = [0x08, 0x09];

    internal Protobuf1CommandHandler CreateSUT()
    {
        WriteCommandCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkWriteCommandCharacteristicId);
        NotifyMessageCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkNotifyMessageCharacteristicId);
        ReadMessageCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkReadMessageCharacteristicId);
        return new Protobuf1CommandHandler(Logger, ProtobufMessageConverter?.Object);
    }

    internal async Task<Protobuf1CommandHandler> CreateConnectedSUT(CommandType commandType = CommandType.Non, bool expectCancelRecordTransfer = false)
    {
        byte[] getGaugeInfoBytes = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06];
        ProtobufMessageConverter.Setup(c => c.ToZippedProtobuf(It.Is<ICommand>(m => m.CommandType == CommandType.GetGaugeInfo))).Returns(getGaugeInfoBytes);
        WriteCommandCharacteristic.Setup(w => w.WriteValueWithResponse(getGaugeInfoBytes)).Returns(Task.CompletedTask);
        ReadMessageCharacteristic.Setup(r => r.ReadValue()).ReturnsAsync(getGaugeInfoBytes);
        V1.Message.GaugeInfo gaugeInfo = new()
        {
            batteryLevel = (uint)Random.Shared.Next(20, 70),
            serialNumber = (uint)Random.Shared.Next(100000, 999999),
            versionNumber = 1,
        };
        ProtobufMessageConverter.Setup(c => c.FromZippedProtoBuf<V1.Message.GaugeInfo>(getGaugeInfoBytes)).Returns(gaugeInfo);
        SendDelayedNotification(CommandType.GetGaugeInfo);

        if (expectCancelRecordTransfer)
        {
            byte[] cancelBytes = [0x01, 0x02, 0x03, 0x04, 0x05, 0x06];
            WriteCommandCharacteristic.Setup(w => w.WriteValueWithResponse(cancelBytes)).Returns(Task.CompletedTask);
            ProtobufMessageConverter.Setup(c => c.ToZippedProtobuf(It.Is<ICommand>(m => m.CommandType == CommandType.CancelRecordTransfer))).Returns(cancelBytes);
        }

        var sut = CreateSUT();
        if (commandType != CommandType.Non)
        {
            byte[] defaultCommandBytes = [0x01, 0x02, 0x03];
            WriteCommandCharacteristic.Setup(w => w.WriteValueWithResponse(defaultCommandBytes)).Returns(Task.CompletedTask);
            ProtobufMessageConverter.Setup(c => c.ToZippedProtobuf(It.Is<ICommand>(m => m.CommandType == commandType))).Returns(defaultCommandBytes);
            byte[] notifyBytes = [0x04, 0x09, 0x05];
            ProtobufMessageConverter.Setup(c => c.FromProtobuf<V1.NotifyMessage>(notifyBytes)).Returns(new V1.NotifyMessage { commandType= (V1.CommandType)commandType });
        }
        
        await sut.Connect(new List<IBLECharacteristic> {
            WriteCommandCharacteristic.Object,
            NotifyMessageCharacteristic.Object,
            ReadMessageCharacteristic.Object,
        });
        return sut;
    }

    public void SendDelayedNotification(CommandType command)
    {
        Task.Delay(100).ContinueWith(
            task =>
            {
                ProtobufMessageConverter.Setup(c => c.FromProtobuf<V1.NotifyMessage>(NotifyBytes)).Returns(new V1.NotifyMessage { commandType = (V1.CommandType)command });
                NotifyMessageCharacteristic.Raise(m => m.CharacteristicValueChanged += null, new BLECharacteristicValueChangedEventArgs { Value = NotifyBytes });
            });
    }
}
