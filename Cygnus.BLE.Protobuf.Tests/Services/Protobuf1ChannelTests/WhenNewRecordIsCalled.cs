using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenNewRecordIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithNewRecordCommandType([Values(RecordType.Linear, RecordType.Grid2D)] RecordType recordType, [Random(3,10,1) ] int measurementCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.NewRecord), false)).ReturnsAsync(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddRecordPoints), false)).ReturnsAsync(true);
        var blankRecord = CreateBlankRecord(recordType, measurementCount);

        // Act
        await sut.NewRecord(blankRecord);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<V1.Command>(m => m.CommandType == CommandType.NewRecord), false), Times.Once);
    }

    [Test]
    public async Task ShouldSendCommandWithAddRecordPointsTheExpectedNumberOfTimes([Values(RecordType.Linear, RecordType.Grid2D)] RecordType recordType, [Random(9, 100, 3)] int measurementCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.NewRecord), false)).ReturnsAsync(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddRecordPoints), false)).ReturnsAsync(true);
        var blankRecord = CreateBlankRecord(recordType, measurementCount);

        // Act
        await sut.NewRecord(blankRecord);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<V1.Command>(m => m.CommandType == CommandType.AddRecordPoints), false), Times.Exactly((measurementCount + 9) / 10));
    }

    [Test]
    public async Task ShouldSendACommandWithExpectedNewRecord([Values(RecordType.Linear, RecordType.Grid2D)] RecordType recordType, [Random(3, 10, 1)] int measurementCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.NewRecord), false)).ReturnsAsync(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<ICommand>(m => m.CommandType == CommandType.AddRecordPoints), false)).ReturnsAsync(true);
        var blankRecord = CreateBlankRecord(recordType, measurementCount);

        // Act
        await sut.NewRecord(blankRecord);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<V1.Command>(m => m.CommandType == CommandType.NewRecord && WithExpectedProperties(m.newRecord, blankRecord)), false), Times.Once);
    }

    private bool WithExpectedProperties(V1.Command.NewRecord newRecord, BlankRecord blankRecord)
    {
        newRecord.recordType.ShouldBe(blankRecord.Type == RecordType.Linear ? V1.RecordType.Linear : V1.RecordType.Grid);
        newRecord.numColsX.ShouldBe(blankRecord.Type == RecordType.Grid2D ? blankRecord.ColumnCount : (uint)blankRecord.MeasurementPoints.Length);
        newRecord.numRowsY.ShouldBe(blankRecord.Type == RecordType.Grid2D ? blankRecord.RowCount : 1);
        newRecord.Key.ShouldBe(blankRecord.Key);
        newRecord.Name.ShouldBe(blankRecord.Name);
        newRecord.Uom.ShouldBe((V1.Uom)blankRecord.Units);
        newRecord.gridPattern.ShouldBe((V1.GridPattern)blankRecord.GridPattern);
        return true;
    }

    private static BlankRecord CreateBlankRecord(RecordType recordType, int measurementCount)
    {
        int index = 0;
        return new()
        {
            Type = recordType,
            ColumnCount = (uint)Random.Shared.Next(3, 10),
            RowCount = (uint)Random.Shared.Next(3, 10),
            Key = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            GridPattern = (GridType)Random.Shared.Next(16),
            Units = Random.Shared.Next(2) == 0 ? MeasurementUnits.Metric : MeasurementUnits.Imperial,
            MeasurementPoints = Enumerable.Range(0, measurementCount)
                .Select(_ => new BlankPoint
                {
                    ColNumX = (uint)(Random.Shared.NextDouble() * 100),
                    RowNumY = (uint)(Random.Shared.NextDouble() * 100),
                    Key = (uint)Random.Shared.Next(1, 1000),
                    Method = recordType == RecordType.BScan ? Method.Spot : Method.Scan,
                    Name = index++.ToString(),
                    ThicknessMaxLimit = (uint)(Random.Shared.NextDouble() * 100),
                    ThicknessMinLimit = (uint)(Random.Shared.NextDouble() * 100),
                }).ToArray()
        };
    }
}
