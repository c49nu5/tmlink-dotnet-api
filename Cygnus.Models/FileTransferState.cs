namespace Cygnus.Models;

public enum FileTransferState
{
    Idle,
    Pending,
    Receiving,
    Sending,
    Deleting,
    FileList,
    Error,
    Complete
}

