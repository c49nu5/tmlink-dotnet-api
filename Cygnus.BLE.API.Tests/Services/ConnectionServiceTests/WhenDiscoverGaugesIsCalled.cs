using Cygnus.BLE.API.Interfaces;
using Moq;

namespace Cygnus.BLE.API.Tests.Services.ConnectionServiceTests;
internal class WhenDiscoverGaugesIsCalled
{
    [Test]
    public async Task ShouldNotifyObserverThatIsScannningIsTrue()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(true, false);
        testBed.Observer.SetupSet(o => o.IsScanning = It.IsAny<bool>());

        // Act
        await sut.DiscoverGauges();

        // Assert
        testBed.Observer.VerifySet(o => o.IsScanning = true, Times.Once);
    }

    [Test]
    public async Task ShouldCallCheckBluetoothConfiguration()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(bluetoothEnabled: false);

        // Act
        await sut.DiscoverGauges();

        // Assert
        testBed.PlatformService.Verify(p => p.CheckBluetoothConfiguration(), Times.Once);
    }

    [Test]
    public async Task AndCheckBluetoothConfigurationReturnsFalse_ShouldNotifyObserverThatIsScannningIsFalse()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(true, false);
        testBed.Observer.SetupSet(o => o.IsScanning = It.IsAny<bool>());

        // Act
        await sut.DiscoverGauges();

        // Assert
        testBed.Observer.VerifySet(o => o.IsScanning = false, Times.Once);
    }

    [Test]
    public async Task AndCheckBluetoothConfigurationReturnsTrue_ShouldCallFindGauges()
    {
        // Arrange
        var testBed = new TestBed();
        testBed.GaugeDiscoverer.Setup(g => g.FindGauges()).ReturnsAsync([]);
        var sut = testBed.CreateSUT(bluetoothEnabled: true);

        // Act
        await sut.DiscoverGauges();

        // Assert
        testBed.GaugeDiscoverer.Verify(g => g.FindGauges(), Times.Once);
    }

    [Test]
    public async Task AndGaugesAreFound_ShouldCallConnectOnEachGauge()
    {
        // Arrange
        var testBed = new TestBed();
        var gauges = Enumerable.Range(1, Random.Shared.Next(1, 20)).Select(_ =>
        {
            Mock<IBLEGauge> gauge = new();
            gauge.Setup(g => g.Connect()).ReturnsAsync(false);
            gauge.Setup(g => g.SerialNumber).Returns(Guid.NewGuid().ToString());
            return gauge;
        }).ToList();
        testBed.GaugeDiscoverer.Setup(g => g.FindGauges()).ReturnsAsync(gauges.Select(g => g.Object));
        var sut = testBed.CreateSUT(bluetoothEnabled: true);

        // Act
        await sut.DiscoverGauges();

        // Assert
        foreach (var gauge in gauges)
        { 
            gauge.Verify(g => g.Connect(), Times.Once); 
        }
    }

    [Test]
    public async Task AndGaugesFoundAreNotConnected_ShouldNotNotifyObserversAboutGauges()
    {
        // Arrange
        var testBed = new TestBed();
        var gauges = Enumerable.Range(1, Random.Shared.Next(1, 20)).Select(_ =>
        {
            Mock<IBLEGauge> gauge = new();
            gauge.Setup(g => g.Connect()).ReturnsAsync(false);
            gauge.Setup(g => g.SerialNumber).Returns(Guid.NewGuid().ToString());
            return gauge;
        }).ToList();
        testBed.GaugeDiscoverer.Setup(g => g.FindGauges()).ReturnsAsync(gauges.Select(g => g.Object));
        var sut = testBed.CreateSUT(true, true);
        testBed.Observer.SetupSet(o => o.IsScanning = It.IsAny<bool>());

        // Act
        await sut.DiscoverGauges();

        // Assert
        testBed.Observer.Verify(o => o.GaugeDiscovered(It.IsAny<IBLEGauge>()), Times.Never);
    }

    [Test]
    public async Task AndGaugesFoundAreConnectedButHaveNoSerialNumber_ShouldNotNotifyObserversAboutGauges()
    {
        // Arrange
        var testBed = new TestBed();
        var gauges = Enumerable.Range(1, Random.Shared.Next(1, 20)).Select(_ =>
        {
            Mock<IBLEGauge> gauge = new();
            gauge.Setup(g => g.Connect()).ReturnsAsync(true);
            return gauge;
        }).ToList();
        testBed.GaugeDiscoverer.Setup(g => g.FindGauges()).ReturnsAsync(gauges.Select(g => g.Object));
        var sut = testBed.CreateSUT(true, true);
        testBed.Observer.SetupSet(o => o.IsScanning = It.IsAny<bool>());

        // Act
        await sut.DiscoverGauges();

        // Assert
        testBed.Observer.Verify(o => o.GaugeDiscovered(It.IsAny<IBLEGauge>()), Times.Never);
    }

    [Test]
    public async Task AndGaugesFoundAreConnectedAndHaveSerialNumber_ShouldNotifyObserversAboutGauges()
    {
        // Arrange
        var testBed = new TestBed();
        var gauges = Enumerable.Range(1, Random.Shared.Next(1, 20)).Select(_ =>
        {
            Mock<IBLEGauge> gauge = new();
            gauge.Setup(g => g.Connect()).ReturnsAsync(true);
            gauge.Setup(g => g.SerialNumber).Returns(Guid.NewGuid().ToString());
            return gauge;
        }).ToList();
        testBed.GaugeDiscoverer.Setup(g => g.FindGauges()).ReturnsAsync(gauges.Select(g => g.Object));
        var sut = testBed.CreateSUT(true, true);
        testBed.Observer.SetupSet(o => o.IsScanning = It.IsAny<bool>());
        testBed.Observer.Setup(o => o.GaugeDiscovered(It.IsAny<IBLEGauge>()));

        // Act
        await sut.DiscoverGauges();

        // Assert
        foreach (var gauge in gauges)
        {
            testBed.Observer.Verify(o => o.GaugeDiscovered(gauge.Object), Times.Once);
        }
    }
}
