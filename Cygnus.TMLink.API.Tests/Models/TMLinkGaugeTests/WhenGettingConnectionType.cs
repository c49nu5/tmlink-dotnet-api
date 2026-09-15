using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

[TestFixture]
internal class WhenGettingConnectionType
{
    [Test]
    public void ShouldReturnM5EX()
    {
        // Arrange
        var tb = new TestBed();
        var sut = tb.CreateSUT();

        // Act
        var result = sut.ConnectionType;

        // Assert
        result.ShouldBe(Cygnus.Models.ConnectionType.TMLink);
    }
}
