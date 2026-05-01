using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.Models;
using InTheHand.Bluetooth;

namespace Cygnus.BLE.API.Interfaces
{
    public interface IBLEGauge : IBLEGaugePresenter, IDisposable
    {
        internal string DeviceIdentifier { get; }

        internal Task<bool> Connect();
        internal IBLEGauge SetDevice(BluetoothDevice device);

        bool IsConnected { get; }

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