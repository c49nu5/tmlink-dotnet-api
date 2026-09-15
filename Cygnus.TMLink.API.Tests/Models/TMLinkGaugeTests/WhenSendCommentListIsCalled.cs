using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

internal class WhenSendCommentListIsCalled
{
    [Test]
    public async Task ShouldThrowNotSupportedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        Action act = () => sut.SendCommentList([]);

        // Assert
        act.ShouldThrow<NotSupportedException>();
    }
}
