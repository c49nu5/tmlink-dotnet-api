using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenCancelRecordTransferIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.CancelRecordTransfer();

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallCancelRecordTransferOnProtobufChannel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.CancelRecordTransfer()).ReturnsAsync(true);

        // Act
        await sut.CancelRecordTransfer();

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.CancelRecordTransfer(), Times.Once);
    }
}
