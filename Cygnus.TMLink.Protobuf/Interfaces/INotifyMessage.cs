namespace Cygnus.TMLink.Protobuf.Interfaces
{

    internal interface INotifyMessage
    {
        CommandType CommandType { get; }
        ErrorCodes ErrorCode { get; }
        bool ReadDataAvailable { get; }
    }
}