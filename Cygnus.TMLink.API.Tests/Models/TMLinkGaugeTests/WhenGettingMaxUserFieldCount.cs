using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

public class WhenGettingMaxUserFieldCount
{
    [Test]
    public void ShouldReturn8()
    {
        // Arrange
        var tb = new TestBed();
        var sut = tb.CreateSUT();

        // Act
        var result = sut.MaxUserFieldCount;

        // Assert
        result.ShouldBe(8);
    }
    }
