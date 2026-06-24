using Cygnus.Interfaces;
using Cygnus.Models;

namespace Cygnus.TMLink.API.Models
{
    internal class TMLinkTransferMonitor : IFileTransferRequest
    {
        private Action<bool> progressUpdater;
        private IFileTransferRequest transferRequest;

        public TMLinkTransferMonitor(Action<bool> progressUpdater, IFileTransferRequest transferRequest)
        {
            this.progressUpdater = progressUpdater;
            this.transferRequest = transferRequest;
        }

        public string Name => transferRequest.Name;

        public double PercentageTransferred { set => transferRequest.PercentageTransferred = value; }

        public FileTransferState Status
        {
            get => transferRequest.Status;
            set
            {
                transferRequest.Status = value;
                progressUpdater(transferRequest.Status is FileTransferState.Sending or FileTransferState.Receiving or FileTransferState.Deleting);
            }
        }

        public RecordType RecordType => transferRequest.RecordType;
    }
}