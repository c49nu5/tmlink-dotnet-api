using Cygnus.TMLink.Interfaces;
using Cygnus.Interfaces;
using Cygnus.Models;

namespace Cygnus.TMLink.Protobuf.Interfaces
{
    public interface IProtobufChannel : IDisposable
    {
        /// <summary>
        /// Indicates whether the channel has been initialized and is ready to use
        /// </summary>
        bool IsInitialized { get; }

        void AddObserver(ILiveMeasurementObserver observer);
        Task<GaugeInformation?> Connect(ITMLinkDevice device);
        void Disconnect();

        Task DeleteAllRecords();
        Task DeleteRecord(IFileTransferRequest deleteRequest);
        Task<GaugeRecord?> GetRecord(IFileTransferRequest transferRequest, bool withAScans);
        Task<List<GaugeRecordSummary>?> GetRecordList();
        Task NewRecord(BlankRecord record);
        Task SubscribeToLiveUpdates();
        void UnsubscribeFromLiveUpdates();

        /// <summary>
        /// Cancel any record transfer currently in progress
        /// </summary>
        /// <returns>true if a transfer was in progress and was cancelled, false otherwise</returns>
        Task<bool> CancelRecordTransfer();
    }
}