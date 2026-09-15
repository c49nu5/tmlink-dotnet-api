using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

internal class WhenSetGaugeTimeIsCalled
{
    [Test]
    public async Task ShouldNotThrowAnException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        Action act = () => sut.SetGaugeTime(DateTime.Now);

        // Assert
        act.ShouldNotThrow();
    }
}
