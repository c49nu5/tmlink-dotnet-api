using Cygnus.Models;

namespace Cygnus.Interfaces;

public interface IDeleteRequest
{
    string Name { get; }
    FileTransferState Status { get; set; }
    RecordType RecordType { get; }
}
