using Cygnus.TMLink.API.Models;
using Cygnus.TMLink.API.Interfaces;
using Cygnus.TMLink.Interfaces;
using Cygnus.TMLink.Protobuf.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Cygnus.Interfaces;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests
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

        public ILogger<TMLinkGauge> Logger { get; set; } = Mock.Of<ILogger<TMLinkGauge>>();
        public Mock<IProtobufChannel> Protobuf1Channel { get; set; } = new Mock<IProtobufChannel>(MockBehavior.Strict);
        public Mock<IProtobufChannel> Protobuf2Channel { get; set; } = new Mock<IProtobufChannel>(MockBehavior.Strict);
        public Mock<ITMLinkConnectionService> ConnectionService { get; set; } = new Mock<ITMLinkConnectionService>(MockBehavior.Strict);
        public Mock<IGaugeMonitor> Observer { get; private set; }
        public Func<byte, IProtobufChannel> ProtobufChannelFactory { get; set; }

        internal TMLinkGauge CreateSUT(bool configureObserver = false)
        {
            Protobuf1Channel.SetupGet(p => p.IsInitialized).Returns(true);
            Protobuf2Channel.SetupGet(p => p.IsInitialized).Returns(true);
            TMLinkGauge gauge = new(Logger, ProtobufChannelFactory, ConnectionService?.Object);
            if (configureObserver)
            {
                Observer = new Mock<IGaugeMonitor>(MockBehavior.Strict);
                gauge.AddObserver(Observer.Object);
            }

            return gauge;
        }

        internal async Task<TMLinkGauge> CreateConnectedSUT()
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

        internal Mock<ITMLinkDevice> CreateDevice(bool isConnectable = false, byte protobufVersion = 1)
        {
            Mock<ITMLinkDevice> mock = new(MockBehavior.Strict);
            mock.Setup(d => d.AddObserver(It.IsAny<ITMLinkDeviceMonitor>()));
            mock.SetupGet(d => d.Id).Returns(Guid.NewGuid().ToString());
            mock.SetupGet(d => d.Name).Returns(Guid.NewGuid().ToString());
            if (isConnectable)
            {
                mock.Setup(d => d.GetCharacteristics(Constants.GenericAccessServiceId)).ReturnsAsync([]);
                mock.Setup(d => d.GetCharacteristics(Constants.DeviceInformationServiceId)).ReturnsAsync([CreateSoftwareVersionCharacteristic(protobufVersion)]);                
                mock.Setup(d => d.Connect()).Returns(Task.CompletedTask);
            }

            return mock;
        }

        private ITMLinkCharacteristic CreateSoftwareVersionCharacteristic(byte protobufVersion)
        {
            var characteristic = new Mock<ITMLinkCharacteristic>(MockBehavior.Strict);
            characteristic.SetupGet(c => c.Uuid).Returns(Constants.SoftwareVersionCharacteristicId);
            characteristic.Setup(c => c.ReadValue()).ReturnsAsync(Encoding.UTF8.GetBytes(protobufVersion.ToString()));
            return characteristic.Object;
        }
    }
}
