using Cygnus.Interfaces;
using Cygnus.Models;

namespace Cygnus.BLE.Interfaces
{
    public interface IBLEGauge : ILiveMeasurementObserver, IDisposable
    {
        bool IsConnected { get; }
        string Name { get; }
        string Model { get; }
        Version? FirmwareVersion { get; }
        string SerialNumber { get; }

        void AddObserver(IGaugeMonitor observer);

        Task<List<GaugeRecordSummary>?> GetRecordList();
        Task<GaugeRecord?> GetRecord(ITransferRequest transferRequest, bool withAScans);
        Task CancelRecordTransfer();
        Task DeleteAllRecords();
        Task DeleteRecord(IDeleteRequest deleteRequest);
        Task NewRecord(BlankRecord record);
        Task SubscribeToLiveUpdates();
        void UnsubscribeFromLiveUpdates();
        void Disconnect();
    }
}