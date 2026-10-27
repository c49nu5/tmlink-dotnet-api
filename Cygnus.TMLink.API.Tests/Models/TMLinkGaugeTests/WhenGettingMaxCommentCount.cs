using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

public class WhenGettingMaxCommentCount
{
    [Test]
    public void ShouldReturn8()
    {
        // Arrange
        var tb = new TestBed();
        var sut = tb.CreateSUT();

        // Act
        var result = sut.MaxCommentCount;

        // Assert
        result.ShouldBe(8);
    }
    }
