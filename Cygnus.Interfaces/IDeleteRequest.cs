using Cygnus.Models;

namespace Cygnus.Interfaces;

public interface IDeleteRequest
{
    string Name { get; }
    TransferStatus Status { get; set; }
    RecordType RecordType { get; }
}
