using Cygnus.TMLink.Protobuf.Interfaces;
using Cygnus.Interfaces;
using Cygnus.Models;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
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
        IFileTransferRequest transferRequestMock = ConfigureTransfer(recordType, testBed, recordName, CommandType.GetRecord);

        // Act
        await sut.GetRecord(transferRequestMock, false);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecord), It.IsAny<CancellationToken>()), Times.Once());
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
        IFileTransferRequest transferRequestMock = ConfigureTransfer(recordType, testBed, recordName, CommandType.GetRecord);

        // Act
        var record = await sut.GetRecord(transferRequestMock, false);

        // Assert
        record.Name.ShouldBe(recordName);
        record.RecordType.ShouldBe(recordType);
    }

    [Test]
    public async Task AndRecordTypeIsGrid2D_ShouldSendACommandWithGetRecordPointCommandTypeCorrectly([Random(10, 15, 1)] int measurementCount, [Values] bool withAScans)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        IFileTransferRequest transferRequestMock = ConfigureTransfer(Models.RecordType.Grid2D, testBed, recordName, CommandType.GetRecord, measurementCount, withAScans);

        // Act
        await sut.GetRecord(transferRequestMock, withAScans);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == (withAScans ? CommandType.GetRecordPointAScan : CommandType.GetRecordPoint)), It.IsAny<CancellationToken>()), Times.Exactly(measurementCount));
    }

    [Test]
    public async Task AndRecordTypeIsLinear_ShouldSendACommandWithGetRecordPointCommandTypeCorrectly([Random(10, 15, 1)] int measurementCount, [Values] bool withAScans)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        IFileTransferRequest transferRequestMock = ConfigureTransfer(Models.RecordType.Linear, testBed, recordName, CommandType.GetRecord, measurementCount, withAScans);

        // Act
        await sut.GetRecord(transferRequestMock, withAScans);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == (withAScans ? CommandType.GetRecordPointAScan : CommandType.GetRecordPoint)), It.IsAny<CancellationToken>()), Times.Exactly(measurementCount));
    }

    [Test]
    public async Task AndRecordTypeIsBScan_ShouldSendACommandWithGetBScanCommandTypeAndName()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        IFileTransferRequest transferRequestMock = ConfigureTransfer(Models.RecordType.BScan, testBed, recordName, CommandType.GetBScan);

        // Act
        await sut.GetRecord(transferRequestMock, false);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScan), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public async Task AndRecordTypeIsBScan_ShouldReturnTheCorrectRecord()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        IFileTransferRequest transferRequestMock = ConfigureTransfer(Models.RecordType.BScan, testBed, recordName, CommandType.GetBScan);

        // Act
        var record = await sut.GetRecord(transferRequestMock, false);

        // Assert
        record.Name.ShouldBe(recordName);
        record.RecordType.ShouldBe(RecordType.BScan);
    }

    [Test]
    public async Task AndRecordTypeIsBScan_ShouldSendACommandWithGetBScanPointCommandTypeCorrectly([Random(10, 15, 1)] int measurementCount, [Values] bool withAScans)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        string recordName = "test_record";
        IFileTransferRequest transferRequestMock = ConfigureTransfer(Models.RecordType.BScan, testBed, recordName, CommandType.GetBScan, measurementCount, withAScans);

        // Act
        await sut.GetRecord(transferRequestMock, withAScans);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == (withAScans ? CommandType.GetBScanPointAScan : CommandType.GetBScanPoint)), It.IsAny<CancellationToken>()), Times.Exactly(measurementCount));
    }

    private static IFileTransferRequest ConfigureTransfer(Models.RecordType recordType, TestBed testBed, string recordName, CommandType command, int measurementCount, bool withAScans)
    {
        uint pointIndex = 0;
        if (recordType == RecordType.BScan)
        {
            var measurementCommand = withAScans ? CommandType.GetBScanPointAScan : CommandType.GetBScanPoint;
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == measurementCommand), It.IsAny<CancellationToken>())).ReturnsAsync(new V1.Message { bscanPoint = new V1.Message.BScanPoint { scanPointNum = pointIndex } });
        }
        else
        {
            var measurementCommand = withAScans ? CommandType.GetRecordPointAScan : CommandType.GetRecordPoint;
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == measurementCommand), It.IsAny<CancellationToken>())).ReturnsAsync(new V1.Message { recordPoint = CreatePoint(pointIndex++) });
        }

        return ConfigureTransfer(recordType, testBed, recordName, command, measurementCount);
    }

    private static IFileTransferRequest ConfigureTransfer(Models.RecordType recordType, TestBed testBed, string recordName, CommandType command, int measurementCount = 0)
    {
        uint pointIndex = 0;
        if (recordType == Models.RecordType.BScan)
        {
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == command), It.IsAny<CancellationToken>())).ReturnsAsync(new V1.Message { Bscan = new V1.Message.BScan { Name = recordName, numScanPoints = (uint)measurementCount } });
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanPoint), It.IsAny<CancellationToken>())).ReturnsAsync(new V1.Message { bscanPoint = new V1.Message.BScanPoint { scanPointNum = pointIndex++, Thickness = 10 * pointIndex, Velocity = 11 * pointIndex } });
        }
        else
        {
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == command), It.IsAny<CancellationToken>())).ReturnsAsync(new V1.Message { record = new V1.Message.Record { Name = recordName, recordType = recordType == RecordType.Linear ? V1.RecordType.Linear : V1.RecordType.Grid, numPointsTaken = (uint)measurementCount, commentList = CreateCommentList(), userFieldValues = CreateUserFieldValues() } });
            testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordPoint), It.IsAny<CancellationToken>())).ReturnsAsync(new V1.Message { recordPoint = CreatePoint(pointIndex++) });
        }

        var transferRequestMock = Mock.Of<IFileTransferRequest>(t => t.Name == recordName && t.RecordType == recordType);
        return transferRequestMock;
    }

    private static V1.Message.Record.UserFieldValues CreateUserFieldValues()
    {
        V1.Message.Record.UserFieldValues userFieldValues = new();
        userFieldValues.Items.AddRange(Enumerable.Range(0, Random.Shared.Next(3)).Select(i => new V1.Message.Record.UserFieldValues.UserFieldValue { ufName = $"uf{i}", ufType = (V1.UserFieldType)Random.Shared.Next(7), ufValue = $"i" }));
        return userFieldValues;
    }

    private static V1.CommentList CreateCommentList()
    {
        V1.CommentList commentList = new();
        commentList.Comments.AddRange(Enumerable.Range(0, Random.Shared.Next(7)).Select(i => $"Com {i}"));
        return commentList;
    }

    private static V1.Message.RecordPoint CreatePoint(uint i)
    {
        return new V1.Message.RecordPoint
        {
            Name = $"p{i}",
            Key = 23121u + i,
            colNumX = i,
            Thickness = 10 * i,
            Velocity = 11 * i,
            Uom = (V1.Uom)Random.Shared.Next(2),
            probeType = (V1.ProbeType)Random.Shared.Next(10),
            commentByte = (uint)Random.Shared.Next(64),
            Ascan = CreateAScan()
        };
    }

    private static V1.AScan CreateAScan()
    {
        V1.AScanSize aScanSize = (V1.AScanSize)Random.Shared.Next(2);
        int numberOfEchos = Random.Shared.Next(4);
        byte[] ascanBytes = aScanSize == V1.AScanSize.U320 ? new byte[320] : new byte[640];
        Random.Shared.NextBytes(ascanBytes);
        return new V1.AScan
        {
            Echo1 = numberOfEchos > 0 ? (uint)Random.Shared.Next(1, 1000) : 0,
            Echo2 = numberOfEchos > 1 ? (uint)Random.Shared.Next(1, 1000) : 0,
            Echo3 = numberOfEchos > 2 ? (uint)Random.Shared.Next(1, 1000) : 0,
            ascanWidth = (uint)Random.Shared.Next(1000, 9000),
            ascanPoints = ascanBytes,
            aScanSize = aScanSize,
            ascanStart = (uint)Random.Shared.Next(0, 100),
            Rectify = (V1.AScanRectify)Random.Shared.Next(0, 4)
        };
    }
}
