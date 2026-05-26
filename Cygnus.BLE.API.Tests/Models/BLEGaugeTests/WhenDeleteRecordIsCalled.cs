using Moq;
using Shouldly;

namespace Cygnus.BLE.API.Tests.Models.BLEGaugeTests;
internal class WhenDeleteRecordIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.DeleteRecord(Mock.Of<Cygnus.Interfaces.IDeleteRequest>());

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallDeleteRecordOnProtobufChannelWithExpectedParameters()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        Cygnus.Interfaces.IDeleteRequest deleteRequest = Mock.Of<Cygnus.Interfaces.IDeleteRequest>();
        testBed.Protobuf1Channel.Setup(p => p.DeleteRecord(deleteRequest)).Returns(Task.CompletedTask);

        // Act
        await sut.DeleteRecord(deleteRequest);

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.DeleteRecord(deleteRequest), Times.Once);
    }
}
