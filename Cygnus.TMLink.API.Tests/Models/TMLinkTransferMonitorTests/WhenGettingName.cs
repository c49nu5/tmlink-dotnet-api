using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkTransferMonitorTests;

internal class WhenGettingName
{
    [Test]
    public void ShouldReturnExpectedValue()
    {
        // Arrange
        var testBed = new TestBed();
        testBed.TransferRequest.SetupGet(t => t.Name).Returns("file1");
        var sut = testBed.CreateSUT();
        // Act
        var name = sut.Name;
        // Assert
        name.ShouldBe("file1");
    }

    [Test]
    public void ShouldReturnNameFromUnderlyingTransferRequest()
    {
        // Arrange
        var testBed = new TestBed();
        testBed.TransferRequest.SetupGet(t => t.Name).Returns("file3");
        var sut = testBed.CreateSUT();
        // Act
        var name = sut.Name;

        // Assert
        testBed.TransferRequest.VerifyGet(t => t.Name, Times.Once);
    }
}