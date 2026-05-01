using Cygnus.BLE.Protobuf.Interfaces;

namespace Cygnus.BLE.Protobuf.V1
{
    public partial class Command : ICommand
    {
        Interfaces.CommandType ICommand.CommandType => (Interfaces.CommandType)commandType;
    }
}
