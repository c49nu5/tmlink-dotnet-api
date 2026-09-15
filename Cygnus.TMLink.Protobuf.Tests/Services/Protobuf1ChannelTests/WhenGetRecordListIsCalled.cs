using Cygnus.TMLink.Protobuf.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenGetRecordListIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithGetRecordListCommandType()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordList))).ReturnsAsync(new V1.Message { recordList = new V1.Message.RecordList() });
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanList))).ReturnsAsync(new V1.Message { bscanList = new V1.Message.BScanList() });

        // Act
        await sut.GetRecordList();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordList)), Times.Once());
    }

    [Test]
    public async Task ShouldSendACommandWithGetBScanListCommandType()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordList))).ReturnsAsync(new V1.Message { recordList = new V1.Message.RecordList() });
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanList))).ReturnsAsync(new V1.Message { bscanList = new V1.Message.BScanList() });

        // Act
        await sut.GetRecordList();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanList)), Times.Once());
    }

    [Test]
    public async Task ShouldReturnTheCorrectRecordListCount([Random(3,20,1)] int recordCount, [Random(3, 20, 1)] int bScanCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        V1.Message expected = new V1.Message { recordList = new V1.Message.RecordList() };
        Enumerable.Range(0, recordCount).ToList().ForEach(i => expected.recordList.Items.Add(new V1.Message.RecordList.Item { Name = $"Record {i}", Created = DateTime.UtcNow, numPointsRequired = 100 }));
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordList))).ReturnsAsync(expected);
        V1.Message expectedBScanList = new V1.Message { bscanList = new V1.Message.BScanList() };
        Enumerable.Range(0, bScanCount).ToList().ForEach(i => expectedBScanList.bscanList.Items.Add(new V1.Message.BScanList.Item { Name = $"BScan {i}", numScanPoints = 2000 }));
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanList))).ReturnsAsync(expectedBScanList);

        // Act
        var recordList = await sut.GetRecordList();

        // Assert
        recordList.Count().ShouldBe(recordCount + bScanCount);
    }

    [Test]
    public async Task WhenRecordListReturnsNull_ShouldReturnTheCorrectRecordListCount([Random(3, 20, 1)] int bScanCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        V1.Message expected = null;
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordList))).ReturnsAsync(expected);
        V1.Message expectedBScanList = new V1.Message { bscanList = new V1.Message.BScanList() };
        Enumerable.Range(0, bScanCount).ToList().ForEach(i => expectedBScanList.bscanList.Items.Add(new V1.Message.BScanList.Item { Name = $"BScan {i}", numScanPoints = 2000 }));
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanList))).ReturnsAsync(expectedBScanList);

        // Act
        var recordList = await sut.GetRecordList();

        // Assert
        recordList.Count().ShouldBe(bScanCount);
    }

    [Test]
    public async Task WhenBScanListReturnsNull_ShouldReturnTheCorrectRecordListCount([Random(3, 20, 1)] int recordCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        V1.Message expected = new V1.Message { recordList = new V1.Message.RecordList() };
        Enumerable.Range(0, recordCount).ToList().ForEach(i => expected.recordList.Items.Add(new V1.Message.RecordList.Item { Name = $"Record {i}", Created = DateTime.UtcNow, numPointsRequired = 100 }));
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordList))).ReturnsAsync(expected);
        V1.Message expectedBScanList = null;
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse<V1.Message>(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanList))).ReturnsAsync(expectedBScanList);

        // Act
        var recordList = await sut.GetRecordList();

        // Assert
        recordList.Count().ShouldBe(recordCount);
    }
}
