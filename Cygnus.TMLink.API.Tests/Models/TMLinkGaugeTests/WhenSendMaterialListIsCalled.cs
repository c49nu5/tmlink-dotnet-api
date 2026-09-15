using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

internal class WhenSendMaterialListIsCalled
{
    [Test]
    public async Task ShouldThrowNotSupportedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        Action act = () => sut.SendMaterialList([]);

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }
}