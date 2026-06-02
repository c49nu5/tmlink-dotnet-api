using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenGetRecordListIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.GetRecordList();

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallGetRecordListOnProtobufChannelWithExpectedParameters()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.GetRecordList()).ReturnsAsync(new List<GaugeRecordSummary>());

        // Act
        await sut.GetRecordList();

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.GetRecordList(), Times.Once);
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldReturnRecordListFromProtobuf1Channel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        List<GaugeRecordSummary> expected = new();
        testBed.Protobuf1Channel.Setup(p => p.GetRecordList()).ReturnsAsync(expected);

        // Act
        var recordList = await sut.GetRecordList();

        // Assert
        recordList.ShouldBe(expected);
    }
}
