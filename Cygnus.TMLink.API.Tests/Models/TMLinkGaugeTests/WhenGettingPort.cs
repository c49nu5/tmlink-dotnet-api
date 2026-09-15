using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

public class WhenGettingPort
{
    [Test]
    public void ShouldReturnBLE()
    {
        // Arrange
        var tb = new TestBed();
        var sut = tb.CreateSUT();

        // Act
        var result = sut.Port;

        // Assert
        result.ShouldBe("BLE");
    }
}
