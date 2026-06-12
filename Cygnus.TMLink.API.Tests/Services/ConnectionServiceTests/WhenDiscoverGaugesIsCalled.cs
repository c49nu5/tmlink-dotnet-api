using Cygnus.Interfaces;
using Cygnus.TMLink.Interfaces;
using Moq;

namespace Cygnus.TMLink.API.Tests.Services.ConnectionServiceTests;
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
        testBed.DeviceDiscoverer.Setup(g => g.FindDevices()).ReturnsAsync([]);
        var sut = testBed.CreateSUT(bluetoothEnabled: true);

        // Act
        await sut.DiscoverGauges();

        // Assert
        testBed.DeviceDiscoverer.Verify(g => g.FindDevices(), Times.Once);
    }

    [Test]
    public async Task AndGaugesAreFound_ShouldCallConnectOnEachGauge()
    {
        // Arrange
        var testBed = new TestBed();
        var devices = Enumerable.Range(1, Random.Shared.Next(1, 20)).Select(i => Mock.Of<ITMLinkDevice>()).ToList();
        testBed.DeviceDiscoverer.Setup(g => g.FindDevices()).ReturnsAsync(devices);
        var sut = testBed.CreateSUT(bluetoothEnabled: true);

        // Act
        await sut.DiscoverGauges();

        // Assert
        foreach (var gauge in testBed.Gauges)
        { 
            gauge.Verify(g => g.Connect(), Times.Once); 
        }
    }

    [Test]
    public async Task AndGaugesFoundAreNotConnected_ShouldNotNotifyObserversAboutGauges()
    {
        // Arrange
        var testBed = new TestBed();
        var devices = Enumerable.Range(1, Random.Shared.Next(1, 20)).Select(i => Mock.Of<ITMLinkDevice>()).ToList();
        testBed.DeviceDiscoverer.Setup(g => g.FindDevices()).ReturnsAsync(devices);
        var sut = testBed.CreateSUT(true, true, false);
        testBed.Observer.SetupSet(o => o.IsScanning = It.IsAny<bool>());

        // Act
        await sut.DiscoverGauges();

        // Assert
        testBed.Observer.Verify(o => o.GaugeDiscovered(It.IsAny<IGauge>()), Times.Never);
    }

    [Test]
    public async Task AndGaugesFoundAreConnectedButHaveNoSerialNumber_ShouldNotNotifyObserversAboutGauges()
    {
        // Arrange
        var testBed = new TestBed();
        var devices = Enumerable.Range(1, Random.Shared.Next(1, 20)).Select(i => Mock.Of<ITMLinkDevice>()).ToList();
        testBed.DeviceDiscoverer.Setup(g => g.FindDevices()).ReturnsAsync(devices);
        var sut = testBed.CreateSUT(true, true, true, false);
        testBed.Observer.SetupSet(o => o.IsScanning = It.IsAny<bool>());

        // Act
        await sut.DiscoverGauges();

        // Assert
        testBed.Observer.Verify(o => o.GaugeDiscovered(It.IsAny<IGauge>()), Times.Never);
    }

    [Test]
    public async Task AndGaugesFoundAreConnectedAndHaveSerialNumber_ShouldNotifyObserversAboutGauges()
    {
        // Arrange
        var testBed = new TestBed();
        var devices = Enumerable.Range(1, Random.Shared.Next(1, 20)).Select(i => Mock.Of<ITMLinkDevice>()).ToList();
        testBed.DeviceDiscoverer.Setup(g => g.FindDevices()).ReturnsAsync(devices);
        var sut = testBed.CreateSUT(true, true);
        testBed.Observer.SetupSet(o => o.IsScanning = It.IsAny<bool>());
        testBed.Observer.Setup(o => o.GaugeDiscovered(It.IsAny<IGauge>()));

        // Act
        await sut.DiscoverGauges();

        // Assert
        foreach (var gauge in testBed.Gauges)
        {
            testBed.Observer.Verify(o => o.GaugeDiscovered(gauge.Object), Times.Once);
        }
    }
}
