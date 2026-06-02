using Cygnus.Models;
using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.ProtobufMessageConverterTests;
internal class WhenFromZippedProtobufIsCalled
{
    [Test]
    public void ShouldReturnObjectWithExpectedProperties()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        V1.Message.Record record = new()
        {
            Created = DateTime.UtcNow,
            Key = Guid.NewGuid().ToString(),
            Location = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            numPointsTaken = (uint)Random.Shared.Next(1, 10),
            numPointsRequired = (uint)Random.Shared.Next(10, 10),
            recordID = (uint)Random.Shared.Next(1, 1000),
            recordType = Random.Shared.Next(0, 2) == 0 ? V1.RecordType.Grid : V1.RecordType.Linear,
            Surveyor = Guid.NewGuid().ToString(),
            Updated = Random.Shared.Next(0, 2) == 0 ? DateTime.UtcNow : null
        };
        var bytes = sut.ToZippedProtobuf(record);

        // Act
        var result = sut.FromZippedProtobuf<V1.Message.Record>(bytes);

        // Assert
        result.Created.ShouldBe(record.Created);
        result.Key.ShouldBe(record.Key);
        result.Location.ShouldBe(record.Location);
        result.Name.ShouldBe(record.Name);
        result.numPointsTaken.ShouldBe(record.numPointsTaken);
        result.numPointsRequired.ShouldBe(record.numPointsRequired);
        result.recordID.ShouldBe(record.recordID);
        result.recordType.ShouldBe(record.recordType);
        result.Surveyor.ShouldBe(record.Surveyor);
        result.Updated.ShouldBe(record.Updated);
    }
}
