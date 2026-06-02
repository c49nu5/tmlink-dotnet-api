using Cygnus.TMLink.Protobuf.Services;

namespace Cygnus.TMLink.Protobuf.Tests.Services.ProtobufMessageConverterTests;
internal class TestBed
{
    internal ProtobufMessageConverter CreateSUT()
    {
        return new ProtobufMessageConverter();
    }
}
