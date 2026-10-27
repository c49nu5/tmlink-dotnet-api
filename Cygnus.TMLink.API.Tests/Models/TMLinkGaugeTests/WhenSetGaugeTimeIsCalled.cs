using Cygnus.Models;
using Cygnus.TMLink.API.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

internal class WhenSetGaugeTimeIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.SetGaugeTime(DateTime.Now);

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallGetGaugeInformationOnProtobufChannel()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Reset();
        DateTime now = DateTime.Now;
        testBed.Protobuf1Channel.Setup(p => p.GetGaugeInformation(now)).ReturnsAsync((GaugeInformation?)null);

        // Act
        var act = sut.SetGaugeTime(now);

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.GetGaugeInformation(now), Times.Once);
    }

}
