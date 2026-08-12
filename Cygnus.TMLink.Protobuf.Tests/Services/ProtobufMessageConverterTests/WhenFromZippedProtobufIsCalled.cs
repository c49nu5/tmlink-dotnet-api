using Cygnus.Models;
using Shouldly;

namespace Cygnus.TMLink.Protobuf.Tests.Services.ProtobufMessageConverterTests;
internal class WhenFromZippedProtobufIsCalled
{
    private const string ZippedProtobufMessageHex =             "1F-8B-08-00-00-00-00-00-04-03-E3-E0-73-9A-CD-CA-C1-2C-D1-32-97-51-61-81-9E-06-87-85-91-53-37-AB-C4-82-5E-36-A5-06-56-06-46-46-86-FF-4C-0C-FF-FE-33-FD-FF-CF-80-0C-18-81-1C-A0-14-90-44-41-FF-99-80-0A-81-72-FF-19-80-04-03-58-0F-23-58-C1-7F-A0-0C-03-AA-11-60-71-14-02-68-19-82-CF-C8-F8-9F-81-89-93-E3-B2-59-B4-DE-7C-07-C7-CB-B7-94-3E-BE-FD-CB-F5-93-89-95-F9-2F-50-19-D0-64-46-26-86-FF-40-35-FF-19-81-C6-02-31-90-06-EA-05-DA-CB-08-64-FD-FB-CF-F2-EF-F7-5F-96-7F-0C-40-A7-03-E5-18-98-FE-03-7D-00-F4-05-50-05-14-81-CC-80-31-A1-34-03-C3-DF-BF-0C-8C-FF-59-59-99-7E-31-32-33-31-00-F5-FD-67-66-F9-F0-5A-45-E7-EA-15-C9-FF-1F-BF-F2-FC-F9-F9-87-E5-DF-5F-06-A6-7F-C0-40-01-D9-0F-F1-18-50-F3-BF-7F-40-D3-80-34-48-E0-DF-DF-7F-C0-20-60-60-61-F9-F3-FB-2F-13-13-D8-8C-7F-BF-FF-B0-FE-FD-03-D4-07-52-05-52-03-54-0A-47-FF-FF-33-82-84-FF-FF-FB-C7-CC-CA-F2-E7-0F-C3-3F-06-D6-DF-3F-FF-B3-FF-FE-0D-74-FA-FF-7F-4C-2C-9F-3E-C9-CB-DF-7E-24-F9-E3-D3-5F-EE-5F-BF-81-3E-85-6B-04-EB-FA-FF-FF-3F-33-F3-DF-3F-40-CC-C0-CC-FC-EF-1F-90-F3-FB-17-0B-2B-E3-7F-26-66-C6-3F-7F-98-18-81-7E-60-60-60-00-3A-99-09-EC-28-A6-7F-FF-80-A6-82-5D-FB-9F-91-89-11-28-0E-D2-03-0C-B2-FF-BF-FF-70-70-B1-7D-E4-16-E6-7E-C3-C3-CF-F2-83-8D-F5-EF-5F-A0-3E-26-A6-7F-FF-FF-FF-F8-21-F6-EB-C5-5B-FE-DF-BF-98-18-FF-B0-B2-32-FC-03-8A-03-6D-06-F9-02-64-0E-23-E3-BF-7F-40-D1-9F-AC-EC-CC-7F-99-99-7F-FF-61-66-F9-FB-17-28-0D-54-F4-1F-12-42-FF-19-80-A6-00-9D-F1-9F-01-18-B2-FF-99-98-FE-B0-B0-32-FD-61-60-60-62-FE-F3-9B-99-05-18-53-FF-FF-32-30-03-ED-FA-FB-8F-FD-37-0B-27-F3-27-6E-A0-27-41-61-CC-FC-07-18-5E-0C-40-93-34-5E-CC-65-04-00-C2-0C-D3-B9-A0-02-00-00";

    [Test]
    public void ShouldReturnBScanPointWithExpectedProperties()
    {
        // Arrange
        var testBed = new TestBed();
        var sut = testBed.CreateSUT();
        var bytes = Array.ConvertAll<string, byte>(ZippedProtobufMessageHex.Split('-'), s => Convert.ToByte(s, 16));

        // Act
        var result = sut.FromZippedProtobuf<V1.Message>(bytes);

        // Assert
        result.bscanPoint.BScanID.ShouldBe(3u);
        result.bscanPoint.scanPointNum.ShouldBe(50u);
    }
}
