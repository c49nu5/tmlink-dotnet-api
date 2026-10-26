using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.API.Tests.Models.TMLinkGaugeTests;

internal class WhenSendMaterialListIsCalled
{
    [Test]
    public async Task AndGaugeHasNotBeenConnected_ShouldThrowNotImplementedException([Random(1, 7, 1)] int materialCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();

        // Act
        var act = sut.SendMaterialList(CreateMaterialList(materialCount));

        // Assert
        await act.ShouldThrowAsync<NotImplementedException>();
    }

    [Test]
    public async Task AndGaugeIsConnected_ShouldCallSendMaterialListOnProtobufChannelWithExpectedParameters([Random(1, 7, 1)] int materialCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.Protobuf1Channel.Setup(p => p.SendMaterialList(It.IsAny<List<Material>>())).Returns(Task.CompletedTask);
        var materialsList = CreateMaterialList(materialCount);

        // Act
        await sut.SendMaterialList(materialsList);

        // Assert
        testBed.Protobuf1Channel.Verify(p => p.SendMaterialList(materialsList), Times.Once);
    }

    private List<Material> CreateMaterialList(int materialCount)
    {
        return [.. Enumerable.Range(0, materialCount).Select(i => new Material { Name = $"Com {i}", Velocity = (uint)i })];
    }
}