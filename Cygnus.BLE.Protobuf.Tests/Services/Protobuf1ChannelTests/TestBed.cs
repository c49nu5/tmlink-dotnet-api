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
    public Mock<ILiveMeasurementObserver> Observer { get; set; } = new Mock<ILiveMeasurementObserver>(MockBehavior.Strict);

    public Mock<IBLECharacteristic> WriteCommandCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> NotifyMessageCharacteristic { get; set; } = new Mock<IBLECharacteristic>();
    public Mock<IBLECharacteristic> ReadMessageCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> LiveCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public Mock<IBLECharacteristic> FrozenCharacteristic { get; set; } = new Mock<IBLECharacteristic>(MockBehavior.Strict);
    public IBLECharacteristic[] Characteristics { get; set; }

    public byte[] CommandBytes = [0x01, 0x02, 0x03, 0x04];
    public byte[] ReadBytes = [0x05, 0x06, 0x07];
    public byte[] NotifyBytes = [0x08, 0x09];
    public byte[] LiveBytes = { 0x11, 0x02, 0x03 };
    public byte[] FrozenBytes = { 0x12, 0x03, 0x03 };

    internal Protobuf1Channel CreateSUT()
    {
        WriteCommandCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkWriteCommandCharacteristicId);
        NotifyMessageCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkNotifyMessageCharacteristicId);
        ReadMessageCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkReadMessageCharacteristicId);
        LiveCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkLiveCharacteristicId);
        FrozenCharacteristic.Setup(c => c.Uuid).Returns(Constants.TMLinkFrozenCharacteristicId);
        Characteristics = CreateTMLinkCharacteristics();
        return new Protobuf1Channel(ProtobufCommandHandler?.Object, Logger, ProtobufMessageConverter?.Object);
    }

    internal async Task<Protobuf1Channel> CreateConnectedSUT(bool expectCancelRecordTransfer = false, bool configureForLiveUpdates = false, bool configureForFrozenUpdates = false)
    {
        if (expectCancelRecordTransfer)
        {
            ProtobufCommandHandler.Setup(c => c.CancelCommand());
            ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.CancelRecordTransfer), true)).ReturnsAsync(true);
        }

        var sut = CreateSUT();

        PrepareForConnect();

        await sut.Connect(Device.Object);

        if (configureForLiveUpdates || configureForFrozenUpdates)
        {
            Observer.Setup(o => o.OnLiveMeasurementReceived(It.Is<Models.LiveMeasurement>(m => m.IsFrozen == configureForFrozenUpdates)));
            sut.AddObserver(Observer.Object);
            LiveCharacteristic.SetupAdd(c => c.CharacteristicValueChanged += null);
            LiveCharacteristic.Setup(c => c.StartNotifications()).Returns(Task.CompletedTask);
            if (configureForFrozenUpdates)
            {
                ProtobufMessageConverter.Setup(c => c.FromProtobuf<V1.NotifyLiveMeasurement>(LiveBytes)).Returns(new V1.NotifyLiveMeasurement { liveMeasurementType = V1.LiveMeasurementType.Frozen, statusBits = (uint)(V1.Constants.IsFrozenFlag | V1.Constants.IsValidFlag) });
                FrozenCharacteristic.Setup(c => c.ReadValue()).ReturnsAsync(FrozenBytes);
                ProtobufMessageConverter.Setup(c => c.FromZippedProtobuf<V1.FrozenLiveMeasurement>(FrozenBytes)).Returns(new V1.FrozenLiveMeasurement { Ascan = new V1.AScan { ascanPoints = [0x04, 0x12, 0x23, 0x04] } });
            }
            else
            {
                ProtobufMessageConverter.Setup(c => c.FromProtobuf<V1.NotifyLiveMeasurement>(LiveBytes)).Returns(new V1.NotifyLiveMeasurement { liveMeasurementType = V1.LiveMeasurementType.Live, statusBits = (uint)(V1.Constants.IsValidFlag) });
            }
        }

        return sut;
    }

    public V1.Message.GaugeInfo PrepareForConnect()
    {
        V1.Message.GaugeInfo gaugeInfo = new()
        {
            batteryLevel = (uint)Random.Shared.Next(20, 70),
            serialNumber = (uint)Random.Shared.Next(100000, 999999),
            versionNumber = 1,
        };

        ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetGaugeInfo), It.IsAny<Func<V1.Message, V1.Message.GaugeInfo>>())).ReturnsAsync(gaugeInfo);

        Device.Setup(d => d.RequestMtuAsync(500)).Returns(Task.CompletedTask);
        Device.SetupGet(g => g.Name).Returns("Test Gauge");

        Device.Setup(d => d.GetCharacteristics(Constants.TMLinkServiceId)).ReturnsAsync(Characteristics);
        ProtobufCommandHandler.Setup(h => h.Connect(Characteristics)).ReturnsAsync(true);
        return gaugeInfo;
    }

    private IBLECharacteristic[] CreateTMLinkCharacteristics()
    {
        return [
            WriteCommandCharacteristic.Object,
            NotifyMessageCharacteristic.Object,
            ReadMessageCharacteristic.Object,
            LiveCharacteristic.Object,
            FrozenCharacteristic.Object,
        ];
    }

    internal void PrepareForDisconnect()
    {
        LiveCharacteristic.SetupRemove(c => c.CharacteristicValueChanged -= null);
        ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.CancelRecordTransfer), true)).ReturnsAsync(true);
        ProtobufCommandHandler.Setup(c => c.Disconnect());
    }
}
