using Shouldly;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
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
    public void ShouldThrowIfProtobufMessageConverterIsNull()
    {
        // Arrange
        var testBed = new TestBed
        {
            ProtobufMessageConverter = null
        };

        // Act
        var getSut = () => testBed.CreateSUT();

        // Assert
        getSut.ShouldThrow<ArgumentNullException>();
    }
}
