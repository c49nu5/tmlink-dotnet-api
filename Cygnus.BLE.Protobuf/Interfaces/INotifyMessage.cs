namespace Cygnus.BLE.Protobuf.Interfaces
{

    internal interface INotifyMessage
    {
        CommandType CommandType { get; }
        ErrorCodes ErrorCode { get; }
        bool ReadDataAvailable { get; }
    }
}