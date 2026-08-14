using Cygnus.TMLink.Protobuf.Interfaces;
using Cygnus.TMLink.Protobuf.Services;
using Microsoft.Extensions.Logging;

namespace Cygnus.TMLink.Protobuf.Tests.Services.Protobuf1ChannelTests
{
    internal class MockProtobuf1Channel : Protobuf1Channel
    {
        public MockProtobuf1Channel(IProtobufCommandHandler protobuf1CommandHandler, ILogger<Protobuf1Channel> logger, IProtobufMessageConverter protobufMessageConverter) : base(protobuf1CommandHandler, logger, protobufMessageConverter)
        {
        }

        public void MockRecordRequest()
        {
            _recordTransferCts = new(TimeSpan.FromSeconds(5));
        }
    }
}