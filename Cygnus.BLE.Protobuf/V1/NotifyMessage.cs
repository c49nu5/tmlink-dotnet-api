using Cygnus.BLE.Protobuf.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Cygnus.BLE.Protobuf.V1
{
    [ExcludeFromCodeCoverage]
    public partial class NotifyMessage : INotifyMessage
    {
        public Interfaces.CommandType CommandType => (Interfaces.CommandType)commandType;
        public Interfaces.ErrorCodes ErrorCode => (Interfaces.ErrorCodes)errorCode;
        public bool ReadDataAvailable => readDataAvailable;
    }
}
