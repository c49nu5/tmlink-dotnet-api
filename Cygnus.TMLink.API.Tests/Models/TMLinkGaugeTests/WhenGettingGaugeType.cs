using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

public class WhenGettingGaugeType
{
    [Test]
    public void ShouldReturnM5EX()
    {
        // Arrange
        var tb = new TestBed();
        var sut = tb.CreateSUT();

        // Act
        var result = sut.GaugeType;

        // Assert
        result.ShouldBe(Cygnus.Models.GaugeType.M5EX);
    }
}
