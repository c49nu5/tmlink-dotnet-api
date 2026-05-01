using Cygnus.BLE.Protobuf.Interfaces;

namespace Cygnus.BLE.Protobuf.V1
{
    public partial class Message : IMessage
    {
        Interfaces.CommandType IMessage.CommandType => (Interfaces.CommandType)commandType;
    }
}
