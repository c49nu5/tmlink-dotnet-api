using Cygnus.BLE.Protobuf.Interfaces;

namespace Cygnus.BLE.Protobuf.V1
{
    public partial class NotifyMessage : INotifyMessage
    {
        Interfaces.CommandType INotifyMessage.CommandType => (Interfaces.CommandType)commandType;
        Interfaces.ErrorCodes INotifyMessage.ErrorCode => (Interfaces.ErrorCodes)errorCode;
        bool INotifyMessage.ReadDataAvailable => readDataAvailable;
    }
}
