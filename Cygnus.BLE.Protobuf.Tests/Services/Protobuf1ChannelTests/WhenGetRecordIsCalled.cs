using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.Interfaces;
using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenGetRecordIsCalled
{
    [Test]
    [TestCase(Models.RecordType.Grid2D)]
    [TestCase(Models.RecordType.Linear)]
    public async Task AndRecordTypeIsRecord_ShouldSendACommandWithGetRecordCommandTypeAndName(Models.RecordType recordType)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        ITransferRequest transferRequestMock = ConfigureTransfer(recordType, testBed, recordName, CommandType.GetRecord);

        // Act
        await sut.GetRecord(transferRequestMock, false);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecord), It.IsAny<Func<V1.Message, V1.Message.Record>>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    [TestCase(Models.RecordType.Grid2D)]
    [TestCase(Models.RecordType.Linear)]
    public async Task AndRecordTypeIsRecord_ShouldReturnTheCorrectRecord(Models.RecordType recordType)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        ITransferRequest transferRequestMock = ConfigureTransfer(recordType, testBed, recordName, CommandType.GetRecord);

        // Act
        var record = await sut.GetRecord(transferRequestMock, false);

        // Assert
        record.Name.ShouldBe(recordName);
        record.RecordType.ShouldBe(recordType);
    }

    [Test]
    public async Task AndRecordTypeIsGrid2D_ShouldSendACommandWithGetRecordPointCommandTypeCorrectly([Random(1, 15, 1)] int measurementCount, [Values] bool withAScans)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        ITransferRequest transferRequestMock = ConfigureTransfer(Models.RecordType.Grid2D, testBed, recordName, CommandType.GetRecord, measurementCount, withAScans);

        // Act
        await sut.GetRecord(transferRequestMock, withAScans);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == (withAScans ? CommandType.GetRecordPointAScan : CommandType.GetRecordPoint)), It.IsAny<Func<V1.Message, V1.Message.RecordPoint>>(), It.IsAny<CancellationToken>()), Times.Exactly(measurementCount));
    }

    [Test]
    public async Task AndRecordTypeIsLinear_ShouldSendACommandWithGetRecordPointCommandTypeCorrectly([Random(1, 15, 1)] int measurementCount, [Values] bool withAScans)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        ITransferRequest transferRequestMock = ConfigureTransfer(Models.RecordType.Linear, testBed, recordName, CommandType.GetRecord, measurementCount, withAScans);

        // Act
        await sut.GetRecord(transferRequestMock, withAScans);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == (withAScans ? CommandType.GetRecordPointAScan : CommandType.GetRecordPoint)), It.IsAny<Func<V1.Message, V1.Message.RecordPoint>>(), It.IsAny<CancellationToken>()), Times.Exactly(measurementCount));
    }

    [Test]
    public async Task AndRecordTypeIsBScan_ShouldSendACommandWithGetBScanCommandTypeAndName()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        ITransferRequest transferRequestMock = ConfigureTransfer(Models.RecordType.BScan, testBed, recordName, CommandType.GetBScan);

        // Act
        await sut.GetRecord(transferRequestMock, false);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScan), It.IsAny<Func<V1.Message, V1.Message.BScan>>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AndRecordTypeIsBScan_ShouldReturnTheCorrectRecord()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        ITransferRequest transferRequestMock = ConfigureTransfer(Models.RecordType.BScan, testBed, recordName, CommandType.GetBScan);

        // Act
        var record = await sut.GetRecord(transferRequestMock, false);

        // Assert
        record.Name.ShouldBe(recordName);
        record.RecordType.ShouldBe(RecordType.BScan);
    }

    [Test]
    public async Task AndRecordTypeIsBScan_ShouldSendACommandWithGetBScanPointCommandTypeCorrectly([Random(1, 15, 1)] int measurementCount, [Values] bool withAScans)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        ITransferRequest transferRequestMock = ConfigureTransfer(Models.RecordType.BScan, testBed, recordName, CommandType.GetBScan, measurementCount, withAScans);

        // Act
        await sut.GetRecord(transferRequestMock, withAScans);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == (withAScans ? CommandType.GetBScanPointAScan : CommandType.GetBScanPoint)), It.IsAny<Func<V1.Message, V1.Message.BScanPoint>>(), It.IsAny<CancellationToken>()), Times.Exactly(measurementCount));
    }

    private static ITransferRequest ConfigureTransfer(Models.RecordType recordType, TestBed testBed, string recordName, CommandType command, int measurementCount, bool withAScans)
    {
        uint pointIndex = 0;
        if (recordType == RecordType.BScan)
        {
            var measurementCommand = withAScans ? CommandType.GetBScanPointAScan : CommandType.GetBScanPoint;
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == measurementCommand), It.IsAny<Func<V1.Message, V1.Message.BScanPoint>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new V1.Message.BScanPoint { scanPointNum = pointIndex });
        }
        else
        {
            var measurementCommand = withAScans ? CommandType.GetRecordPointAScan : CommandType.GetRecordPoint;
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == measurementCommand), It.IsAny<Func<V1.Message, V1.Message.RecordPoint>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new V1.Message.RecordPoint { Name = recordName + pointIndex++, Key = 23121u + pointIndex });
        }

        return ConfigureTransfer(recordType, testBed, recordName, command, measurementCount);
    }

    private static ITransferRequest ConfigureTransfer(Models.RecordType recordType, TestBed testBed, string recordName, CommandType command, int measurementCount = 0)
    {
        if (recordType == Models.RecordType.BScan)
        {
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == command), It.IsAny<Func<V1.Message, V1.Message.BScan>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new V1.Message.BScan{ Name = recordName, numScanPoints = (uint)measurementCount });
        }
        else
        {
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == command), It.IsAny<Func<V1.Message, V1.Message.Record>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new V1.Message.Record { Name = recordName, recordType = (V1.RecordType)recordType, numPointsTaken = (uint)measurementCount });
        }

        var transferRequestMock = Mock.Of<ITransferRequest>(t => t.Name == recordName && t.RecordType == recordType);
        return transferRequestMock;
    }
}
