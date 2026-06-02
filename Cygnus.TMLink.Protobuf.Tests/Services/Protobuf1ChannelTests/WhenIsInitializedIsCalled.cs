using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenIsInitializedIsCalled
{
    [Test]
    public void ShouldReturnTrueAsync()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var isInitialized = sut.IsInitialized;

        // Assert
        isInitialized.ShouldBeTrue();
    }
}
