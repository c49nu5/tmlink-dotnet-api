using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkTransferMonitorTests;

internal class WhenGettingRecordType
{
    [Test]
    public void ShouldReturnExpectedRecordType([Values] RecordType expectedRecordType)
    {
        // Arrange
        var testBed = new TestBed();
        testBed.TransferRequest.SetupGet(t => t.RecordType).Returns(expectedRecordType);

        var sut = testBed.CreateSUT();

        // Act / Assert
        RecordType recordType = sut.RecordType;

        // Assert
        recordType.ShouldBe(expectedRecordType);
    }

    [Test]
    public void ShouldForwardToUnderlyingTransferRequest([Values] RecordType expectedRecordType)
    {
        // Arrange
        var testBed = new TestBed();
        testBed.TransferRequest.SetupGet(t => t.RecordType).Returns(expectedRecordType);

        var sut = testBed.CreateSUT();

        // Act / Assert
        RecordType recordType = sut.RecordType;

        // Assert
        testBed.TransferRequest.VerifyGet(t => t.RecordType, Times.Once);
    }
}