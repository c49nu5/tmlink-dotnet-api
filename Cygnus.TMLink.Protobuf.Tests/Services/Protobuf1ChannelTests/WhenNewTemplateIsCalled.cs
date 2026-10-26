using Cygnus.TMLink.Protobuf.Interfaces;
using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenNewTemplateIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithNewTemplateCommandType([Random(3,10,1) ] int measurementCount, [Random(0,2,1)] int userFieldCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.NewTemplate), false)).ReturnsAsync(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddTemplatePoints), false)).ReturnsAsync(true);
        if (userFieldCount > 0)
        {
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddTemplateUserField), false)).ReturnsAsync(true);
        }
        var blankTemplate = CreateBlankTemplate(measurementCount, userFieldCount);

        // Act
        await sut.NewTemplate(blankTemplate);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<V1.Command>(m => m.CommandType == CommandType.NewTemplate), false), Times.Once);
    }

    [Test]
    public async Task ShouldSendCommandWithAddTemplatePointsTheExpectedNumberOfTimes([Random(9, 100, 3)] int measurementCount, [Random(0, 4, 1)] int userFieldCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.NewTemplate), false)).ReturnsAsync(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddTemplatePoints), false)).ReturnsAsync(true);
        if (userFieldCount > 0)
        {
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddTemplateUserField), false)).ReturnsAsync(true);
        }
        var blankTemplate = CreateBlankTemplate(measurementCount, userFieldCount);

        // Act
        await sut.NewTemplate(blankTemplate);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<V1.Command>(m => m.CommandType == CommandType.AddTemplatePoints), false), Times.Exactly((measurementCount + 9) / 10));
    }

    [Test]
    public async Task ShouldSendCommandWithAddUserFieldTheExpectedNumberOfTimes([Random(9, 100, 3)] int measurementCount, [Random(0, 4, 1)] int userFieldCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.NewTemplate), false)).ReturnsAsync(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddTemplatePoints), false)).ReturnsAsync(true);
        if (userFieldCount > 0)
        {
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddTemplateUserField), false)).ReturnsAsync(true);
        }
        var blankTemplate = CreateBlankTemplate(measurementCount, userFieldCount);

        // Act
        await sut.NewTemplate(blankTemplate);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<V1.Command>(m => m.CommandType == CommandType.AddTemplateUserField), false), Times.Exactly(userFieldCount));
    }


    [Test]
    public async Task ShouldSendACommandWithExpectedNewTemplate([Random(3, 10, 1)] int measurementCount, [Random(0, 4, 1)] int userFieldCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.NewTemplate), false)).ReturnsAsync(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddTemplatePoints), false)).ReturnsAsync(true);
        if (userFieldCount > 0)
        {
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddTemplateUserField), false)).ReturnsAsync(true);
        }

        var blankTemplate = CreateBlankTemplate(measurementCount, userFieldCount);

        // Act
        await sut.NewTemplate(blankTemplate);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<V1.Command>(m => m.CommandType == CommandType.NewTemplate && WithExpectedProperties(m.newTemplate, blankTemplate)), false), Times.Once);
    }

    private bool WithExpectedProperties(V1.Command.NewTemplate newTemplate, BlankTemplate blankTemplate)
    {
        newTemplate.numMPoints.ShouldBe((uint)blankTemplate.MeasurementPoints.Length);
        newTemplate.Key.ShouldBe(blankTemplate.Key);
        newTemplate.Name.ShouldBe(blankTemplate.Name);
        newTemplate.Uom.ShouldBe((V1.Uom)blankTemplate.Units);
        newTemplate.Velocity.ShouldBe(blankTemplate.Velocity);
        return true;
    }

    private static BlankTemplate CreateBlankTemplate(int measurementCount, int userFieldCount)
    {
        int index = 0;
        return new()
        {
            Key = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            Units = Random.Shared.Next(2) == 0 ? MeasurementUnits.Metric : MeasurementUnits.Imperial,
            Velocity = (uint)Random.Shared.Next(3, 100),
            MeasurementPoints = Enumerable.Range(0, measurementCount)
                .Select(_ => new BlankPoint
                {
                    ColNumX = (uint)(Random.Shared.NextDouble() * 100),
                    RowNumY = (uint)(Random.Shared.NextDouble() * 100),
                    Key = (uint)Random.Shared.Next(1, 1000),
                    Name = index++.ToString(),
                    ThicknessMaxLimit = (uint)(Random.Shared.NextDouble() * 100),
                    ThicknessMinLimit = (uint)(Random.Shared.NextDouble() * 100),
                }).ToArray(),
            UserFields = Enumerable.Range(0, userFieldCount)
                .Select(i => new UserField
                {
                    List = Random.Shared.Next(3) == 0 ? ["fdsfs", "fdsfds"] : [],
                    Name = $"User field {i}",
                    Type = (UserFieldType)Random.Shared.Next(6)
                }).ToArray()
        };
    }
}
