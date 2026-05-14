using Shouldly;

namespace Cygnus.BLE.API.Tests.Services.ConnectionServiceTests;
internal class WhenConstructed
{
    [Test]
    public void ShouldReturnAnInstance()
    {
        // Arrange
        var testBed = new TestBed();

        // Act
        var sut = testBed.CreateSUT();

        // Assert
        sut.ShouldNotBeNull();
    }

    [Test]
    public void ShouldThrowIfLoggerIsNull()
    {
        // Arrange
        var testBed = new TestBed
        {
            Logger = null
        };

        // Act
        var getSut = () => testBed.CreateSUT();

        // Assert
        getSut.ShouldThrow<ArgumentNullException>();
    }

    [Test]
    public void ShouldThrowIfPlatformServiceIsNull()
    {
        // Arrange
        var testBed = new TestBed
        {
            PlatformService = null
        };

        // Act
        var getSut = () => testBed.CreateSUT();

        // Assert
        getSut.ShouldThrow<ArgumentNullException>();
    }

    [Test]
    public void ShouldThrowIfGaugeDiscovererIsNull()
    {
        // Arrange
        var testBed = new TestBed
        {
            GaugeDiscoverer = null
        };

        // Act
        var getSut = () => testBed.CreateSUT();

        // Assert
        getSut.ShouldThrow<ArgumentNullException>();
    }
}
