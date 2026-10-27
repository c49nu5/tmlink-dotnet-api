using Cygnus.Interfaces;
using Cygnus.Models;
using Cygnus.TMLink.API.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenGetRecordIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.GetRecord(Mock.Of<IFileTransferRequest>(), true);

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallGetRecordOnProtobufChannelWithExpectedParameters([Values] bool withAScans, [Values] bool withWiderAScans)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(withWiderAScans);
        IFileTransferRequest transferRequest = Mock.Of<IFileTransferRequest>();
        testBed.Protobuf1Channel.Setup(p => p.GetRecord(It.IsAny<TMLinkTransferMonitor>(), withAScans, withWiderAScans)).ReturnsAsync(new GaugeRecord());

        // Act
        await sut.GetRecord(transferRequest, withAScans);

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.GetRecord(It.IsAny<TMLinkTransferMonitor>(), withAScans, withWiderAScans), Times.Once);
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
    public async Task AndTransferProgresses_ShouldSetIsDataTransferInProgress(FileTransferState status, bool inProgress)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Observer.Setup(o => o.OnPropertiesUpdated(sut));
        IFileTransferRequest transferRequest = Mock.Of<IFileTransferRequest>();
        IFileTransferRequest monitor = null;
        testBed.Protobuf1Channel.Setup(p => p.GetRecord(It.IsAny<TMLinkTransferMonitor>(), false, false)).Callback<IFileTransferRequest, bool, bool>((m,a,b) => monitor = m).ReturnsAsync(new GaugeRecord());
        sut.IsDataTransferInProgress = !inProgress;
        await sut.GetRecord(transferRequest, false);

        // Act
        monitor.Status = status;

        // Assert
        sut.IsDataTransferInProgress.ShouldBe(inProgress);
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldReturnRecordFromProtobuf1Channel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        IFileTransferRequest transferRequest = Mock.Of<IFileTransferRequest>();
        GaugeRecord expected = new();
        testBed.Protobuf1Channel.Setup(p => p.GetRecord(It.IsAny<TMLinkTransferMonitor>(), false, false)).ReturnsAsync(expected);

        // Act
        var record = await sut.GetRecord(transferRequest, false);

        // Assert
        record.ShouldBe(expected);
    }
}
