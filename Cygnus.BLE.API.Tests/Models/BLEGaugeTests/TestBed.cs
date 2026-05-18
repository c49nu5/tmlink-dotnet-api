using Cygnus.BLE.API.Interfaces;
using Cygnus.BLE.API.Models;
using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace Cygnus.BLE.API.Tests.Models.BLEGaugeTests
{
    internal class TestBed
    {
        public TestBed()
        {
            ProtobufChannelFactory = (byte value) =>
            {
                return value switch
                {
                    1 => Protobuf1Channel.Object,
                    2 => Protobuf2Channel.Object,
                    _ => null,
                };
            };
        }

        public ILogger<BLEGauge> Logger { get; set; } = Mock.Of<ILogger<BLEGauge>>();
        public Mock<IProtobufChannel> Protobuf1Channel { get; set; } = new Mock<IProtobufChannel>(MockBehavior.Strict);
        public Mock<IProtobufChannel> Protobuf2Channel { get; set; } = new Mock<IProtobufChannel>(MockBehavior.Strict);
        public Mock<IConnectionService> ConnectionService { get; set; } = new Mock<IConnectionService>(MockBehavior.Strict);
        public Mock<IGaugeMonitor> Observer { get; private set; }
        public Func<byte, IProtobufChannel> ProtobufChannelFactory { get; set; }

        internal BLEGauge CreateSUT(bool configureObserver = false)
        {
            Protobuf1Channel.SetupGet(p => p.IsInitialized).Returns(true);
            Protobuf2Channel.SetupGet(p => p.IsInitialized).Returns(true);
            BLEGauge bLEGauge = new(Logger, ProtobufChannelFactory, ConnectionService?.Object);
            if (configureObserver)
            {
                Observer = new Mock<IGaugeMonitor>(MockBehavior.Strict);
                bLEGauge.AddObserver(Observer.Object);
            }

            return bLEGauge;
        }

        internal async Task<BLEGauge> CreateConnectedSUT()
        {
            var sut = CreateSUT(true);
            var device = CreateDevice(true);
            Protobuf1Channel.Setup(p => p.Connect(device.Object)).ReturnsAsync(new Cygnus.Models.GaugeInformation { SerialNumber = (uint)Random.Shared.Next(23132,41232)});
            Protobuf1Channel.Setup(p => p.AddObserver(sut));
            device.Setup(d => d.IsConnected).Returns(true);
            sut.SetDevice(device.Object);
            await sut.Connect();
            return sut;
        }

        internal Mock<IBLEDevice> CreateDevice(bool isConnectable = false, byte protobufVersion = 1)
        {
            Mock<IBLEDevice> mock = new(MockBehavior.Strict);
            mock.Setup(d => d.AddObserver(It.IsAny<IBLEDeviceMonitor>()));
            mock.SetupGet(d => d.Id).Returns(Guid.NewGuid().ToString());
            mock.SetupGet(d => d.Name).Returns(Guid.NewGuid().ToString());
            if (isConnectable)
            {
                mock.Setup(d => d.GetCharacteristics(Constants.GenericAccessServiceId)).ReturnsAsync(new Dictionary<string, IBLECharacteristic>());
                var deviceCharacteristics = new Dictionary<string, IBLECharacteristic>
                {
                    { Constants.SoftwareVersionCharacteristicId, CreateSoftwareVersionCharacteristic(protobufVersion) }
                };
                mock.Setup(d => d.GetCharacteristics(Constants.DeviceInformationServiceId)).ReturnsAsync(deviceCharacteristics);                
                mock.Setup(d => d.Connect()).Returns(Task.CompletedTask);
            }

            return mock;
        }

        private IBLECharacteristic CreateSoftwareVersionCharacteristic(byte protobufVersion)
        {
            var characteristic = new Mock<IBLECharacteristic>(MockBehavior.Strict);
            characteristic.Setup(c => c.ReadValue()).ReturnsAsync(Encoding.UTF8.GetBytes(protobufVersion.ToString()));
            return characteristic.Object;
        }
    }
}
