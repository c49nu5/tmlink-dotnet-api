using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

internal class WhenSendVelocityIsCalled
{
    [Test]
    public async Task ShouldThrowNotSupportedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        Action act = () => sut.SendVelocity(432u, Cygnus.Models.MeasurementUnits.Default);

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }
}
