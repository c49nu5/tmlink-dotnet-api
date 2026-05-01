using Cygnus.Models;

namespace Cygnus.BLE.Protobuf.Interfaces;

public interface IDeleteRequest
{
    string Name { get; }
    TransferStatus Status { get; set; }
    RecordType RecordType { get; }
}
