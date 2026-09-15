using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

internal class WhenDoProbeZeroIsCalled
{
    [Test]
    public async Task ShouldThrowNotSupportedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        Action act = () => sut.DoProbeZero();

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }
}
