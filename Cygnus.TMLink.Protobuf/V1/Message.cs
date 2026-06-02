using Cygnus.TMLink.Protobuf.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Cygnus.TMLink.Protobuf.V1
{
    [ExcludeFromCodeCoverage]
    public partial class Message : IMessage
    {
        public Interfaces.CommandType CommandType => (Interfaces.CommandType)commandType;
    }
}
