using Cygnus.Models;
using Moq;

namespace Cygnus.TMLink.API.Tests.Services.ConnectionServiceTests;
internal class WhenCancelDiscoverIsCalled
{
    [Test]
    public void ShouldCallCancelOnDeviceDiscoverer()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(bluetoothEnabled: false);
        testBed.DeviceDiscoverer.Setup(d => d.Cancel());

        // Act
        sut.CancelDiscover();

        // Assert
        testBed.DeviceDiscoverer.Verify(d => d.Cancel(), Times.Once);
    }

    [Test]
    public async Task ShouldNotifyObserverThatIsScannningIsFalse()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT(true, false);
        testBed.Observer.SetupSet(o => o.ConnectionState = It.IsAny<ConnectionState>());
        testBed.DeviceDiscoverer.Setup(d => d.Cancel());

        // Act
        sut.CancelDiscover();

        // Assert
        testBed.Observer.VerifySet(o => o.ConnectionState = ConnectionState.Disconnected, Times.Once);
    }
}
