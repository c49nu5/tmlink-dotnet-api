namespace Cygnus.BLE.Protobuf.Interfaces
{
    public enum ErrorCodes
    {
        Success = 0,
        RecordNotFound = 1,
        BScanNotFound = 2,
        InvalidParameter = 3,
        TransferNotStarted = 4,
        GzUncompressError = 5,
        ProtoUnpackError = 6,
        MemoryError = 7,
        RecordSaveError = 8,
        RecordNameError = 9,
        RecordZeroPoints = 10,
        RecordPointNumError = 11,
        TransferCancelled = 12,
    }
}