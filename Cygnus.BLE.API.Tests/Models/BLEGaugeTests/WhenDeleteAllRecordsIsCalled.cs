using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.BLE.API.Tests.Models.BLEGaugeTests;
internal class WhenDeleteAllRecordsIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.DeleteAllRecords();

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallDeleteAllRecordsOnProtobufChannel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.DeleteAllRecords()).Returns(Task.CompletedTask);

        // Act
        await sut.DeleteAllRecords();

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.DeleteAllRecords(), Times.Once);
    }
}
