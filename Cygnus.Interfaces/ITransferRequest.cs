using Cygnus.Models;

namespace Cygnus.Interfaces;

public interface ITransferRequest
{
    string Name { get; }
    // A value between 0.0 and 1.0 that specifies the fraction of the measurements that have transferred
    double PercentageTransferred { set; }
    FileTransferState Status { get; set; }
    RecordType RecordType { get; }
}
