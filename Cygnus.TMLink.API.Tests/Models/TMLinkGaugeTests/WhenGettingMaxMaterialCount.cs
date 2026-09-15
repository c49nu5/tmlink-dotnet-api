using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

public class WhenGettingMaxMaterialCount
{
    [Test]
    public void ShouldReturn100()
    {
        // Arrange
        var tb = new TestBed();
        var sut = tb.CreateSUT();

        // Act
        var result = sut.MaxMaterialCount;

        // Assert
        result.ShouldBe(100);
    }
    }
