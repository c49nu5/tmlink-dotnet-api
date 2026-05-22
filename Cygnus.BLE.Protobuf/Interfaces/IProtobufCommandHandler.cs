using Cygnus.BLE.Interfaces;
using Cygnus.BLE.Protobuf.V1;

namespace Cygnus.BLE.Protobuf.Interfaces
{
    public interface IProtobufCommandHandler
    {
        Task<bool> Connect(IEnumerable<IBLECharacteristic> characteristics);
        void Disconnect();

        Task<T?> SendCommandWithResponse<T, M>(ICommand gaugeCommand, Func<M, T> responseHandler, CancellationToken? token = null)
            where T : class
            where M : IMessage;
        Task<bool> SendCommand(ICommand gaugeCommand, bool ignoreErrors = false);
        void CancelCommand();
    }
}