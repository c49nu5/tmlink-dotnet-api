using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Cygnus.BLE.Protobuf.Tests.Services.ProtobufMessageConverterTests;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class TestBed
{
    public ILogger<Protobuf1Channel> Logger { get; set; } = Mock.Of<ILogger<Protobuf1Channel>>();
    public Mock<IProtobufMessageConverter> ProtobufMessageConverter { get; set; } = new Mock<IProtobufMessageConverter>(MockBehavior.Strict);
    public Mock<IBLEDevice> Device { get; set; } = new Mock<IBLEDevice>(MockBehavior.Strict);
    public Mock<IBLEGaugePresenter> GaugePresenter { get; set; } = new Mock<IBLEGaugePresenter>(MockBehavior.Strict);

    internal Protobuf1Channel CreateSUT()
    {
        return new Protobuf1Channel(Logger, ProtobufMessageConverter?.Object);
    }

    internal async Task<Protobuf1Channel> CreateConnectedSUT(bool configureForCommand = true)
    {
        var sut = CreateSUT();
        Device.Setup(d => d.RequestMtuAsync(500)).Returns(Task.CompletedTask);
        if (configureForCommand)
        {
            Device.Setup(d => d.GetCharacteristics(Constants.TMLinkServiceId)).ReturnsAsync(CreateTMLinkCharacteristics);
        }
        
        await sut.Connect(Device.Object, GaugePresenter.Object);
        return sut;
    }

    private IDictionary<string, IBLECharacteristic> CreateTMLinkCharacteristics()
    {
        return new Dictionary<string, IBLECharacteristic>();
    }
}
