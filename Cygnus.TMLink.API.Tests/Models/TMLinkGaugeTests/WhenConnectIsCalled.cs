using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests
{
    [TestFixture]
    internal class WhenConnectIsCalled
    {
        [Test]
        public async Task WhenWithProtobufChannelFactoryReturningNull_ReturnsFalse()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true);
            tb.ConnectionService.Setup(s => s.GaugeIsDisconnected(It.IsAny<string>()));

            sut.SetDevice(device.Object);

            // Act
            var result = await sut.Connect();

            // Assert
            result.ShouldBeFalse();
        }

        [Test]
        public async Task WhenProtobufConnectFails_ReturnsFalse()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true);

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(false);
            tb.Protobuf1Channel.Setup(p => p.Dispose());

            sut.SetDevice(device.Object);

            // Act
            var result = await sut.Connect();

            // Assert
            result.ShouldBeFalse();
        }

        [Test]
        public async Task WhenProtobufConnectFails_SetsIsConnectedToFalse()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true);

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(false);
            tb.Protobuf1Channel.Setup(p => p.Dispose());

            sut.SetDevice(device.Object);

            // Act
            await sut.Connect();

            // Assert
            sut.IsConnected.ShouldBeFalse();
        }

        [Test]
        public async Task WhenProtobufConnectFails_DisposesChannel()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true, true);

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(false);
            tb.Protobuf1Channel.Setup(p => p.Dispose());
            tb.Protobuf1Channel.Setup(p => p.AddObserver(sut));

            sut.SetDevice(device.Object);

            // Act
            await sut.Connect();

            // Assert
            tb.Protobuf1Channel.Verify(p => p.Dispose(), Times.Once);
        }

        [Test]
        public async Task WhenGetGaugeInformationReturnsNull_ReturnsFalse()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true);

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(true);
            tb.Protobuf1Channel.Setup(p => p.GetGaugeInformation()).ReturnsAsync((GaugeInformation?)null);
            tb.Protobuf1Channel.Setup(p => p.Dispose());

            sut.SetDevice(device.Object);

            // Act
            var result = await sut.Connect();

            // Assert
            result.ShouldBeFalse();
        }

        [Test]
        public async Task WhenGetGaugeInformationReturnsNull_SetsIsConnectedToFalse()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true);

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(true);
            tb.Protobuf1Channel.Setup(p => p.GetGaugeInformation()).ReturnsAsync((GaugeInformation?)null);
            tb.Protobuf1Channel.Setup(p => p.Dispose());

            sut.SetDevice(device.Object);

            // Act
            await sut.Connect();

            // Assert
            sut.IsConnected.ShouldBeFalse();
        }

        [Test]
        public async Task WhenGetGaugeInformationReturnsNull_DisposesChannel()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true, true);

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(true);
            tb.Protobuf1Channel.Setup(p => p.GetGaugeInformation()).ReturnsAsync((GaugeInformation?)null);
            tb.Protobuf1Channel.Setup(p => p.Dispose());
            tb.Protobuf1Channel.Setup(p => p.AddObserver(sut));

            sut.SetDevice(device.Object);

            // Act
            await sut.Connect();

            // Assert
            tb.Protobuf1Channel.Verify(p => p.Dispose(), Times.Once);
        }

        [Test]
        public async Task WhenPopulatesSerialNumber()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true, true);

            var gaugeInfo = new GaugeInformation { SerialNumber = 12345 };

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(true);
            tb.Protobuf1Channel.Setup(p => p.GetGaugeInformation()).ReturnsAsync(gaugeInfo);
            tb.Protobuf1Channel.Setup(p => p.AddObserver(sut));

            sut.SetDevice(device.Object);

            // Act
            await sut.Connect();

            // Assert
            sut.SerialNumber.ShouldBe(12345u);
        }

        [Test]
        public async Task WhenPopulatesSoftwareVersionNumber()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true, true);

            var gaugeInfo = new GaugeInformation { SoftwareVersionNumber = 2 };

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(true);
            tb.Protobuf1Channel.Setup(p => p.GetGaugeInformation()).ReturnsAsync(gaugeInfo);
            tb.Protobuf1Channel.Setup(p => p.AddObserver(sut));

            sut.SetDevice(device.Object);

            // Act
            await sut.Connect();

            // Assert
            sut.SoftwareVersionNumber.ShouldBe(2u);
        }

        [Test]
        public async Task WhenPopulatesGaugeVariant()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true, true);

            var gaugeInfo = new GaugeInformation { GaugeVariant = Cygnus.Models.GaugeVariant.BasicSC };

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(true);
            tb.Protobuf1Channel.Setup(p => p.GetGaugeInformation()).ReturnsAsync(gaugeInfo);
            tb.Protobuf1Channel.Setup(p => p.AddObserver(sut));

            sut.SetDevice(device.Object);

            // Act
            await sut.Connect();

            // Assert
            sut.GaugeVariant.ShouldBe(Cygnus.Models.GaugeVariant.BasicSC);
        }

        [Test]
        public async Task WhenPopulatesBatteryLevel()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true, true);

            var gaugeInfo = new GaugeInformation { BatteryLevel = 75 };

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(true);
            tb.Protobuf1Channel.Setup(p => p.GetGaugeInformation()).ReturnsAsync(gaugeInfo);
            tb.Protobuf1Channel.Setup(p => p.AddObserver(sut));

            sut.SetDevice(device.Object);

            // Act
            await sut.Connect();

            // Assert
            sut.BatteryLevel.ShouldBe(75u);
        }

        [Test]
        public async Task WhenPopulatesGaugeId()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true, true);

            var gaugeInfo = new GaugeInformation { GaugeId = 999 };

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(true);
            tb.Protobuf1Channel.Setup(p => p.GetGaugeInformation()).ReturnsAsync(gaugeInfo);
            tb.Protobuf1Channel.Setup(p => p.AddObserver(sut));

            sut.SetDevice(device.Object);

            // Act
            await sut.Connect();

            // Assert
            sut.GaugeId.ShouldBe(999u);
        }

        [Test]
        public async Task WhenPopulatesProbe()
        {
            // Arrange
            var tb = new TestBed();
            var sut = tb.CreateSUT(true);
            var device = tb.CreateDevice(true, true);

            var gaugeInfo = new GaugeInformation { ProbeType = Cygnus.Models.ProbeType.T5A };

            tb.Protobuf1Channel.Setup(p => p.Connect(device.Object, sut)).ReturnsAsync(true);
            tb.Protobuf1Channel.Setup(p => p.GetGaugeInformation()).ReturnsAsync(gaugeInfo);
            tb.Protobuf1Channel.Setup(p => p.AddObserver(sut));

            sut.SetDevice(device.Object);

            // Act
            await sut.Connect();

            // Assert
            sut.ProbeType.ShouldBe(Cygnus.Models.ProbeType.T5A);
        }
    }
}