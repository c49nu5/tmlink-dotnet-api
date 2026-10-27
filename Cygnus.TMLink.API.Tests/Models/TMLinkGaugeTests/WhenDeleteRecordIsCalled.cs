using Cygnus.Interfaces;
using Cygnus.Models;
using Cygnus.TMLink.API.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenDeleteRecordIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.DeleteRecord(Mock.Of<IFileTransferRequest>());

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallDeleteRecordOnProtobufChannelWithExpectedParameters()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        IFileTransferRequest deleteRequest = Mock.Of<IFileTransferRequest>();
        testBed.Protobuf1Channel.Setup(p => p.DeleteRecord(It.IsAny<TMLinkTransferMonitor>())).Returns(Task.CompletedTask);

        // Act
        await sut.DeleteRecord(deleteRequest);

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.DeleteRecord(It.IsAny<TMLinkTransferMonitor>()), Times.Once);
    }

    [Test]
    [TestCase(FileTransferState.Complete, false)]
    [TestCase(FileTransferState.Deleting, true)]
    [TestCase(FileTransferState.Error, false)]
    [TestCase(FileTransferState.FileList, false)]
    [TestCase(FileTransferState.Idle, false)]
    [TestCase(FileTransferState.Pending, false)]
    [TestCase(FileTransferState.Receiving, true)]
    [TestCase(FileTransferState.Sending, true)]
    public async Task AndDeleteProgresses_ShouldSetIsDataTransferInProgress(FileTransferState status, bool inProgress)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Observer.Setup(o => o.OnPropertiesUpdated(sut));
        IFileTransferRequest deleteRequest = Mock.Of<IFileTransferRequest>();
        IFileTransferRequest monitor = null;
        testBed.Protobuf1Channel.Setup(p => p.DeleteRecord(It.IsAny<TMLinkTransferMonitor>())).Callback<IFileTransferRequest>(m => monitor = m).Returns(Task.CompletedTask);
        sut.IsDataTransferInProgress = !inProgress;
        await sut.DeleteRecord(deleteRequest);

        // Act
        monitor.Status = status;

        // Assert
        sut.IsDataTransferInProgress.ShouldBe(inProgress);
    }
}
