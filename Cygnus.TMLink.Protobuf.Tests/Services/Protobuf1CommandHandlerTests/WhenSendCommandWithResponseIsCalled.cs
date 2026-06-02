using Cygnus.TMLink.Interfaces;
using Cygnus.TMLink.Protobuf.Services;
using Cygnus.TMLink.Protobuf.V1;
using Moq;
using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1CommandHandlerTests;
internal class WhenSendCommandWithResponseIsCalled
{
    [Test]
    public async Task ShouldReturnTheExpectedValueFromTheReadCharacteristic()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.ConfigureCommand(CommandType.GetRecord);
        testBed.ReadMessageCharacteristic.Setup(c => c.ReadValue()).ReturnsAsync(testBed.ReadBytes);
        Message.Record expectedValue = new() { recordType = RecordType.Linear };
        testBed.ProtobufMessageConverter.Setup(c => c.FromZippedProtobuf<Message>(testBed.ReadBytes)).Returns(new Message { commandType = CommandType.GetRecord, record = expectedValue });

        // Act
        var result = await sut.SendCommandWithResponse<Message.Record, Message>(new Command { commandType = CommandType.GetRecord }, m => m.record);

        // Assert
        result.ShouldBe(expectedValue);
    }

    [Test]
    public async Task ShouldReturnNullIfCancellationTokenIsCancelled()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = await testBed.CreateConnectedSUT();
        testBed.ConfigureCommand(CommandType.GetRecord, 500);
        testBed.ReadMessageCharacteristic.Setup(c => c.ReadValue()).ReturnsAsync(testBed.ReadBytes);
        Message.Record expectedValue = new() { recordType = RecordType.Linear };
        testBed.ProtobufMessageConverter.Setup(c => c.FromZippedProtobuf<Message>(testBed.ReadBytes)).Returns(new Message { commandType = CommandType.GetRecord, record = expectedValue });
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(200);

        // Act
        var result = await sut.SendCommandWithResponse<Message.Record, Message>(new Command { commandType = CommandType.GetRecord }, m => m.record, cancellationTokenSource.Token);

        // Assert
        result.ShouldBe(null);
    }
}
