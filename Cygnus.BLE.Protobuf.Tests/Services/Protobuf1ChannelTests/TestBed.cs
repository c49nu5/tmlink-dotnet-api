using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class TestBed
{
    public ILogger<Protobuf1Channel> Logger { get; set; } = Mock.Of<ILogger<Protobuf1Channel>>();
    public Mock<IProtobufMessageConverter> ProtobufMessageConverter { get; set; } = new Mock<IProtobufMessageConverter>(MockBehavior.Strict);
    public Mock<IBLEDevice> Device { get; set; } = new Mock<IBLEDevice>(MockBehavior.Strict);
    public Mock<ILiveMeasurementObserver> GaugePresenter { get; set; } = new Mock<ILiveMeasurementObserver>(MockBehavior.Strict);

    public Mock<IBLECharacteristic> WriteCommandCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> MessageReadyCharacteristic { get; set; } = new Mock<IBLECharacteristic>();
    public Mock<IBLECharacteristic> ReadMessageCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> LiveCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> FrozenCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);


    internal Protobuf1Channel CreateSUT()
    {
        return new Protobuf1Channel(Logger, ProtobufMessageConverter?.Object);
    }

    internal Task<Protobuf1Channel> CreateConnectedSUT(CommandType commandType = CommandType.Non)
    {
        return CreateConnectedSUT<object>(commandType);
    }

    internal async Task<Protobuf1Channel> CreateConnectedSUT<T>(CommandType commandType = CommandType.Non) where T : class, new()

    {
        var sut = CreateSUT();
        Device.Setup(d => d.RequestMtuAsync(500)).Returns(Task.CompletedTask);
        Device.SetupGet(g => g.Name).Returns("Test Gauge");
        if (commandType != CommandType.Non)
        {
            byte[] commandBytes = [0x01, 0x02, 0x03];
            WriteCommandCharacteristic.Setup(w => w.WriteValueWithResponse(commandBytes)).Returns(Task.CompletedTask);
            ProtobufMessageConverter.Setup(c => c.ToZippedProtobuf(It.Is<ICommand>(m => m.CommandType == commandType))).Returns(commandBytes);
            byte[] notifyBytes = [0x04, 0x05, 0x05];
            ProtobufMessageConverter.Setup(c => c.FromProtobuf<V1.NotifyMessage>(notifyBytes)).Returns(new V1.NotifyMessage { commandType= (V1.CommandType)commandType });
            ReadMessageCharacteristic.Setup(r => r.ReadValue()).ReturnsAsync(notifyBytes);
            ProtobufMessageConverter.Setup(c => c.FromZippedProtoBuf<T>(commandBytes)).Returns(new T());
            Device.Setup(d => d.GetCharacteristics(Constants.TMLinkServiceId)).ReturnsAsync(CreateTMLinkCharacteristics());
        }
        
        await sut.Connect(Device.Object);
        return sut;
    }

    private IDictionary<string, IBLECharacteristic> CreateTMLinkCharacteristics()
    {
        return new Dictionary<string, IBLECharacteristic> {
            { Constants.TMLinkWriteCommandCharacteristicId, WriteCommandCharacteristic.Object },
            { Constants.TMLinkMessageReadyCharacteristicId, MessageReadyCharacteristic.Object },
            { Constants.TMLinkReadMessageCharacteristicId, ReadMessageCharacteristic.Object },
            { Constants.TMLinkLiveCharacteristicId, LiveCharacteristic.Object },
            { Constants.TMLinkFrozenCharacteristicId, FrozenCharacteristic.Object },
        };
    }
}
