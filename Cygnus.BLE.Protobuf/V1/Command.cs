using Cygnus.BLE.Protobuf.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Cygnus.BLE.Protobuf.V1
{
    [ExcludeFromCodeCoverage]
    public partial class Command : ICommand
    {
        public Interfaces.CommandType CommandType => (Interfaces.CommandType)commandType;
    }
}
