using Cygnus.BLE.Interfaces;
using Cygnus.Interfaces;
using Cygnus.Models;

namespace Cygnus.BLE.Protobuf.Interfaces
{
    public interface IProtobufChannel : IDisposable
    {
        bool IsInitialized { get; }

        Task CancelRecordTransfer();
        Task Connect(IBLEDevice device, IBLEGaugePresenter gaugeInformation);
        Task DeleteAllRecords();
        Task DeleteRecord(IDeleteRequest deleteRequest);
        void Disconnect();
        Task<GaugeRecord?> GetRecord(ITransferRequest transferRequest, bool withAScans);
        Task<List<GaugeRecordSummary>?> GetRecordList();
        Task NewRecord(BlankRecord record);
        Task SubscribeToLiveUpdates();
        void UnsubscribeFromLiveUpdates();
    }
}