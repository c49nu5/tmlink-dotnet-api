using Shouldly;

namespace Cygnus.BLE.API.Tests.Models.BLEGaugeTests;
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
    public void ShouldThrowIfProtobufChannelFactoryIsNull()
    {
        // Arrange
        var testBed = new TestBed
        {
            ProtobufChannelFactory = null
        };

        // Act
        var getSut = () => testBed.CreateSUT();

        // Assert
        getSut.ShouldThrow<ArgumentNullException>();
    }

    [Test]
    public void ShouldThrowIfConnectionServiceIsNull()
    {
        // Arrange
        var testBed = new TestBed
        {
            ConnectionService = null
        };

        // Act
        var getSut = () => testBed.CreateSUT();

        // Assert
        getSut.ShouldThrow<ArgumentNullException>();
    }
}
