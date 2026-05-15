using Cygnus.BLE.Protobuf.Services;

namespace Cygnus.BLE.Protobuf.Tests.Services.ProtobufMessageConverterTests;
internal class TestBed
{
    internal ProtobufMessageConverter CreateSUT()
    {
        return new ProtobufMessageConverter();
    }
}
