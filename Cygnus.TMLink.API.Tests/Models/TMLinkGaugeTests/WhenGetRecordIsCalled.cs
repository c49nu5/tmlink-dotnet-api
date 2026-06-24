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
        var act = sut.GetRecord(Mock.Of<Cygnus.Interfaces.IFileTransferRequest>(), true);

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallGetRecordOnProtobufChannelWithExpectedParameters([Values] bool withAScans)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        Cygnus.Interfaces.IFileTransferRequest transferRequest = Mock.Of<Cygnus.Interfaces.IFileTransferRequest>();
        testBed.Protobuf1Channel.Setup(p => p.GetRecord(It.IsAny<TMLinkTransferMonitor>(), withAScans)).ReturnsAsync(new GaugeRecord());

        // Act
        await sut.GetRecord(transferRequest, withAScans);

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.GetRecord(It.IsAny<TMLinkTransferMonitor>(), withAScans), Times.Once);
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldReturnRecordFromProtobuf1Channel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        Cygnus.Interfaces.IFileTransferRequest transferRequest = Mock.Of<Cygnus.Interfaces.IFileTransferRequest>();
        GaugeRecord expected = new();
        testBed.Protobuf1Channel.Setup(p => p.GetRecord(It.IsAny<TMLinkTransferMonitor>(), false)).ReturnsAsync(expected);

        // Act
        var record = await sut.GetRecord(transferRequest, false);

        // Assert
        record.ShouldBe(expected);
    }
}
