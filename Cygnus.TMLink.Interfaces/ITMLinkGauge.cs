using Cygnus.Interfaces;
using Cygnus.Models;

namespace Cygnus.TMLink.Interfaces
{
    public interface ITMLinkGauge : IDisposable
    {
        bool IsConnected { get; }
        string Name { get; }
        string Model { get; }
        Version? FirmwareVersion { get; }
        string SerialNumber { get; }

        void AddObserver(ITMLinkGaugeMonitor observer);

        Task<List<GaugeRecordSummary>?> GetRecordList();
        Task<GaugeRecord?> GetRecord(IFileTransferRequest transferRequest, bool withAScans);
        Task CancelRecordTransfer();
        Task DeleteAllRecords();
        Task DeleteRecord(IFileTransferRequest deleteRequest);
        Task NewRecord(BlankRecord record);
        Task SubscribeToLiveUpdates();
        void UnsubscribeFromLiveUpdates();
        void Disconnect();
    }
}