using Cygnus.BLE.Protobuf.Interfaces;
using Moq;
using Shouldly;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenGetRecordListIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithGetRecordListCommandType()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordList), It.IsAny<Func<V1.Message, V1.Message.RecordList>>())).ReturnsAsync(new V1.Message.RecordList());
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanList), It.IsAny<Func<V1.Message, V1.Message.BScanList>>())).ReturnsAsync(new V1.Message.BScanList());

        // Act
        await sut.GetRecordList();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordList), It.IsAny<Func<V1.Message, V1.Message.RecordList>>()), Times.Once());
    }

    [Test]
    public async Task ShouldSendACommandWithGetBScanListCommandType()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordList), It.IsAny<Func<V1.Message, V1.Message.RecordList>>())).ReturnsAsync(new V1.Message.RecordList());
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanList), It.IsAny<Func<V1.Message, V1.Message.BScanList>>())).ReturnsAsync(new V1.Message.BScanList());

        // Act
        await sut.GetRecordList();

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanList), It.IsAny<Func<V1.Message, V1.Message.BScanList>>()), Times.Once());
    }

    [Test]
    public async Task ShouldReturnTheCorrectRecordListCount([Random(3,20,1)] int recordCount, [Random(3, 20, 1)] int bScanCount)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        V1.Message.RecordList expected = new();
        Enumerable.Range(0, recordCount).ToList().ForEach(i => expected.Items.Add(new V1.Message.RecordList.Item { Name = $"Record {i}", Created = DateTime.UtcNow, numPointsRequired = 100 }));
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetRecordList), It.IsAny<Func<V1.Message, V1.Message.RecordList>>())).ReturnsAsync(expected);
        V1.Message.BScanList expectedBScanList = new();
        Enumerable.Range(0, bScanCount).ToList().ForEach(i => expectedBScanList.Items.Add(new V1.Message.BScanList.Item { Name = $"BScan {i}", numScanPoints = 2000 }));
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommandWithResponse(It.Is<ICommand>(m => m.CommandType == CommandType.GetBScanList), It.IsAny<Func<V1.Message, V1.Message.BScanList>>())).ReturnsAsync(expectedBScanList);

        // Act
        var recordList = await sut.GetRecordList();

        // Assert
        recordList.Count().ShouldBe(recordCount + bScanCount);
    }
}
