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
    public Mock<IProtobufCommandHandler> ProtobufCommandHandler { get; set; } = new Mock<IProtobufCommandHandler>(MockBehavior.Strict);
    public Mock<IBLEDevice> Device { get; set; } = new Mock<IBLEDevice>(MockBehavior.Strict);
    public Mock<ILiveMeasurementObserver> GaugePresenter { get; set; } = new Mock<ILiveMeasurementObserver>(MockBehavior.Strict);

    public Mock<IBLECharacteristic> WriteCommandCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> NotifyMessageCharacteristic { get; set; } = new Mock<IBLECharacteristic>();
    public Mock<IBLECharacteristic> ReadMessageCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> LiveCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> FrozenCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public IEnumerable<IBLECharacteristic> Characteristics { get; private set; }

    public byte[] CommandBytes = [0x01, 0x02, 0x03, 0x04];
    public byte[] ReadBytes = [0x05, 0x06, 0x07];
    public byte[] NotifyBytes = [0x08, 0x09];

    internal Protobuf1Channel CreateSUT()
    {
        WriteCommandCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkWriteCommandCharacteristicId);
        NotifyMessageCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkNotifyMessageCharacteristicId);
        ReadMessageCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkReadMessageCharacteristicId);
        LiveCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkLiveCharacteristicId);
        FrozenCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkFrozenCharacteristicId);
        return new Protobuf1Channel(ProtobufCommandHandler?.Object, Logger, ProtobufMessageConverter?.Object);
    }

    internal async Task<Protobuf1Channel> CreateConnectedSUT(bool expectCancelRecordTransfer = false)
    {
        V1.Message.GaugeInfo gaugeInfo = new()
        {
            batteryLevel = (uint)Random.Shared.Next(20, 70),
            serialNumber = (uint)Random.Shared.Next(100000, 999999),
            versionNumber = 1,
        };

        ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetGaugeInfo), It.IsAny<Func<V1.Message, V1.Message.GaugeInfo>>())).ReturnsAsync(gaugeInfo);

        if (expectCancelRecordTransfer)
        {
            ProtobufCommandHandler.Setup(c => c.CancelCommand());
            ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.CancelRecordTransfer), true)).Returns(Task.CompletedTask);
        }

        var sut = CreateSUT();
        Device.Setup(d => d.RequestMtuAsync(500)).Returns(Task.CompletedTask);
        Device.SetupGet(g => g.Name).Returns("Test Gauge");

        Characteristics = CreateTMLinkCharacteristics();
        Device.Setup(d => d.GetCharacteristics(Constants.TMLinkServiceId)).ReturnsAsync(Characteristics);
        ProtobufCommandHandler.Setup(h => h.Connect(Characteristics)).ReturnsAsync(true);
        
        await sut.Connect(Device.Object);
        return sut;
    }

    private IEnumerable<IBLECharacteristic> CreateTMLinkCharacteristics()
    {
        return new List<IBLECharacteristic> {
            WriteCommandCharacteristic.Object,
            NotifyMessageCharacteristic.Object,
            ReadMessageCharacteristic.Object,
            LiveCharacteristic.Object,
            FrozenCharacteristic.Object,
        };
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
