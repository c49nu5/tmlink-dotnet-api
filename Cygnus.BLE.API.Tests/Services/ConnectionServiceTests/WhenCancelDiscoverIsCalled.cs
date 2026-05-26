using Moq;

namespace Cygnus.BLE.API.Tests.Services.ConnectionServiceTests;
internal class WhenCancelDiscoverIsCalled
{
    [Test]
    public void ShouldCallCancelOnGaugeDiscoverer()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(bluetoothEnabled: false);
        testBed.GaugeDiscoverer.Setup(d => d.Cancel());

        // Act
        sut.CancelDiscover();

        // Assert
        testBed.GaugeDiscoverer.Verify(d => d.Cancel(), Times.Once);
    }

    [Test]
    public async Task ShouldNotifyObserverThatIsScannningIsFalse()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(true, false);
        testBed.Observer.SetupSet(o => o.IsScanning = It.IsAny<bool>());
        testBed.GaugeDiscoverer.Setup(d => d.Cancel());

        // Act
        sut.CancelDiscover();

        // Assert
        testBed.Observer.VerifySet(o => o.IsScanning = false, Times.Once);
    }
}
