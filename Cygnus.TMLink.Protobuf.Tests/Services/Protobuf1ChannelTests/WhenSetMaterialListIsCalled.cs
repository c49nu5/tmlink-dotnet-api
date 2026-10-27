using Cygnus.Models;
using Cygnus.TMLink.Protobuf.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenSetMaterialListIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithSetMaterialListCommandType([Random(1, 7, 1)] int materialCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.SetMaterialList), false)).ReturnsAsync(true);
        var materialsList = CreateMaterialList(materialCount);

        // Act
        await sut.SendMaterialList(materialsList);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.SetMaterialList), false), Times.Once);
    }

    [Test]
    public async Task ShouldSendACommandWithExpectedMaterials([Random(1, 7, 1)] int materialCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.SetMaterialList), false)).ReturnsAsync(true);
        var materialsList = CreateMaterialList(materialCount);

        // Act
        await sut.SendMaterialList([..materialsList]);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<V1.Command>(m => MatchingCommand(m, materialsList)), false), Times.Once);
    }

    private bool MatchingCommand(V1.Command m, List<Material> materialsList)
    {
        m.materialList.materialLists.Select(m => m.Name).ToArray().ShouldBeEquivalentTo(materialsList.Select(m => m.Name).ToArray()); 
        return true;
    }

    private List<Material> CreateMaterialList(int materialCount)
    {
        return [.. Enumerable.Range(0, materialCount).Select(i => new Material { Name = $"Com {i}", Velocity = (uint)i })];
    }
}
