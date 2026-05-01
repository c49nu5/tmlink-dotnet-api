namespace Cygnus.BLE.Protobuf.Interfaces
{
    public enum CommandType
    {
        Non = 0,
        GetGaugeInfo = 1,
        CancelRecordTransfer = 2,
        GetRecordList = 3,
        GetRecord = 4,
        GetRecordPoint = 5,
        GetRecordPointAScan = 6,
        DeleteRecord = 7,
        DeleteAllRecords = 8,
        NewRecord = 9,
        AddRecordPoints = 10,
        GetBScanList = 11,
        GetBScan = 12,
        GetBScanPoint = 13,
        GetBScanPointAScan = 14,
        DeleteBScan = 15,
        DeleteAllBScans = 16,
    }
}