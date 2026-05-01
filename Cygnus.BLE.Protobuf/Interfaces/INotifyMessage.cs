namespace Cygnus.BLE.Protobuf.Interfaces
{

    public interface INotifyMessage
    {
        CommandType CommandType { get; }
        ErrorCodes ErrorCode { get; }
        bool ReadDataAvailable { get; }
    }
}