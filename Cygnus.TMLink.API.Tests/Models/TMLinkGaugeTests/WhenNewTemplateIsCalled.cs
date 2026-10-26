using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;
internal class WhenNewTemplateIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.NewTemplate(new BlankTemplate());

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallNewTemplateOnProtobufChannelWithExpectedParameters()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        BlankTemplate blankTemplate = new();
        testBed.Protobuf1Channel.Setup(p => p.NewTemplate(blankTemplate)).Returns(Task.CompletedTask);

        // Act
        await sut.NewTemplate(blankTemplate);

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.NewTemplate(blankTemplate), Times.Once);
    }
}
