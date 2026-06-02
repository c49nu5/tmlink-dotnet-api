using Cygnus.TMLink.Protobuf.Interfaces;
using Cygnus.TMLink.Protobuf.V1;
using Microsoft.Extensions.Logging;
namespace Cygnus.TMLink.Protobuf.Services
{
    internal class Protobuf1CommandHandler : ProtobufCommandHandler<NotifyMessage>
    {
        public Protobuf1CommandHandler(ILogger<Protobuf1CommandHandler> logger, IProtobufMessageConverter protobufMessageConverter) 
            : base(logger, protobufMessageConverter)
        {
            logger.LogInformation("Creating Protobuf1CommandHandler");
        }
    }
}