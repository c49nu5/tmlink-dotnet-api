using Cygnus.TMLink.Interfaces;
using Cygnus.Interfaces;
using Cygnus.Models;

namespace Cygnus.TMLink.Protobuf.Interfaces
{
    public interface IProtobufChannel : IDisposable
    {
        bool IsInitialized { get; }

        void AddObserver(ILiveMeasurementObserver observer);
        Task CancelRecordTransfer();
        Task<GaugeInformation?> Connect(ITMLinkDevice device);
        Task DeleteAllRecords();
        Task DeleteRecord(IFileTransferRequest deleteRequest);
        void Disconnect();
        Task<GaugeRecord?> GetRecord(IFileTransferRequest transferRequest, bool withAScans);
        Task<List<GaugeRecordSummary>?> GetRecordList();
        Task NewRecord(BlankRecord record);
        Task SubscribeToLiveUpdates();
        void UnsubscribeFromLiveUpdates();
    }
}