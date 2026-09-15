using Cygnus.TMLink.Interfaces;

namespace Cygnus.TMLink.Protobuf.Interfaces
{
    internal interface IProtobufCommandHandler
    {
        Task<bool> Connect(ITMLinkCharacteristic[] characteristics);
        void Disconnect();

        Task<M?> SendCommandWithResponse<M>(ICommand gaugeCommand, CancellationToken? token = null)
            where M : class, IMessage;
        Task<bool> SendCommand(ICommand gaugeCommand, bool ignoreErrors = false);
        void CancelCommand();
    }
}