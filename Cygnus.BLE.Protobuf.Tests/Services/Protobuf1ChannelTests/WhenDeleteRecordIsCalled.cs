using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.Services;
using Cygnus.Interfaces;
using Moq;

namespace Cygnus.BLE.Protobuf.Tests.Services.Protobuf1ChannelTests;
internal class WhenDeleteRecordIsCalled
{
    [Test]
    public async Task ShouldSendACommandWithDeleteAllRecordsCommandType([Values]Models.RecordType recordType)
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT(true);
        var recordName = Guid.NewGuid().ToString();
        var deleteRequest = ConfigureRequest(recordType, testBed, recordName);

        // Act
        await sut.DeleteRecord(deleteRequest);

        // Assert
        testBed.ProtobufCommandHandler.Verify(c => c.SendCommand(It.Is<V1.Command>(m => m.commandType == (recordType == Models.RecordType.BScan ? V1.CommandType.DeleteBScan : V1.CommandType.DeleteRecord) && m.Name == recordName), false), Times.Once);
    }

    private static IDeleteRequest ConfigureRequest(Models.RecordType recordType, TestBed testBed, string recordName, int measurementCount = 0)
    {
        testBed.ProtobufCommandHandler.Setup(c => c.SendCommand(It.Is<V1.Command>(m => m.commandType == (recordType == Models.RecordType.BScan ? V1.CommandType.DeleteBScan : V1.CommandType.DeleteRecord) && m.Name == recordName), false)).ReturnsAsync(true);
        var transferRequestMock = Mock.Of<IDeleteRequest>(t => t.Name == recordName && t.RecordType == recordType);
        return transferRequestMock;
    }
}
