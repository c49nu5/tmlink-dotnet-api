using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.V1;
using Microsoft.Extensions.Logging;
namespace Cygnus.BLE.Protobuf.Services
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