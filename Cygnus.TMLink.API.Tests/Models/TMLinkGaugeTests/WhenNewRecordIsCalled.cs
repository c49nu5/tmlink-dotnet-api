using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenNewRecordIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.NewRecord(new BlankRecord());

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallNewRecordOnProtobufChannelWithExpectedParameters()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        BlankRecord blankRecord = new();
        testBed.Protobuf1Channel.Setup(p => p.NewRecord(blankRecord)).Returns(Task.CompletedTask);

        // Act
        await sut.NewRecord(blankRecord);

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.NewRecord(blankRecord), Times.Once);
    }
}
