using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenDeleteAllTemplatesIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.DeleteAllTemplates();

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallDeleteAllRecordsOnProtobufChannel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.DeleteAllTemplates()).Returns(Task.CompletedTask);

        // Act
        await sut.DeleteAllTemplates();

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.DeleteAllTemplates(), Times.Once);
    }
}
