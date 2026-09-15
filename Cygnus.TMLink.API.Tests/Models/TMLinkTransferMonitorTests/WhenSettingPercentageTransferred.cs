using Moq;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkTransferMonitorTests;

internal class WhenSettingPercentageTransferred
{
    [Test]
    public void ShouldDelegateToTransferRequest([Random(0.0, 1.0, 1)] double percentage)
    {
        // Arrange
        var testBed = new TestBed();
        testBed.TransferRequest.SetupSet(t => t.PercentageTransferred = It.IsAny<double>());
        var sut = testBed.CreateSUT();

        // Act
        sut.PercentageTransferred = percentage;

        // Assert
        testBed.TransferRequest.VerifySet(t => t.PercentageTransferred = percentage, Times.Once);
    }
}