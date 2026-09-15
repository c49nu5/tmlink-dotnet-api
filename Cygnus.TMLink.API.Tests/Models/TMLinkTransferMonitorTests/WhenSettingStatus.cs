using Cygnus.Models;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkTransferMonitorTests;

internal class WhenSettingStatus
{
    [Test]
    [TestCase(FileTransferState.Receiving)]
    [TestCase(FileTransferState.Sending)]
    [TestCase(FileTransferState.Deleting)]
    [TestCase(FileTransferState.Idle)]
    [TestCase(FileTransferState.Complete)]
    [TestCase(FileTransferState.Error)]
    public void SettingStatusShouldUpdateTransferRequest(FileTransferState expectedValue)
    {
        // Arrange
        var testBed = new TestBed();
        // Allow the mock to store the Status property
        testBed.TransferRequest.SetupProperty(t => t.Status, FileTransferState.Idle);

        var sut = testBed.CreateSUT();

        // Act
        sut.Status = expectedValue;

        // Assert
        sut.Status.ShouldBe(expectedValue);
    }

    [Test]
    [TestCase(FileTransferState.Receiving)]
    [TestCase(FileTransferState.Sending)]
    [TestCase(FileTransferState.Deleting)]
    public void SettingStatusToSendingReceivingDeletingShouldInvokeProgressUpdaterWithTrue(FileTransferState expectedValue)
    {
        // Arrange
        var testBed = new TestBed();
        // Allow the mock to store the Status property
        testBed.TransferRequest.SetupProperty(t => t.Status, FileTransferState.Idle);

        var sut = testBed.CreateSUT();

        // Act
        sut.Status = expectedValue;

        // Assert
        testBed.ProgressUpdates.ShouldNotBeEmpty();
        testBed.ProgressUpdates.First().ShouldBeTrue();
    }

    [Test]
    [TestCase(FileTransferState.Idle)]
    [TestCase(FileTransferState.Complete)]
    [TestCase(FileTransferState.Error)]
    public void SettingStatusToNonTransferStateShouldInvokeProgressUpdaterWithFalse(FileTransferState expectedValue)
    {
        // Arrange
        var testBed = new TestBed();
        testBed.TransferRequest.SetupProperty(t => t.Status, FileTransferState.Sending);

        var sut = testBed.CreateSUT();

        // clear any previous updates, then set to non-transfer state
        testBed.ProgressUpdates.Clear();

        // Act
        sut.Status = expectedValue;

        // Assert
        testBed.TransferRequest.Object.Status.ShouldBe(expectedValue);
        testBed.ProgressUpdates.ShouldNotBeEmpty();
        testBed.ProgressUpdates.First().ShouldBeFalse();
    }
}
