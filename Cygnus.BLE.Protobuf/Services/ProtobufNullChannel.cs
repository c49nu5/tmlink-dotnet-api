using Cygnus.Models;
using Cygnus.BLE.Protobuf.Interfaces;
using InTheHand.Bluetooth;

namespace Cygnus.BLE.Protobuf.Services
{
    public class ProtobufNullChannel : IProtobufChannel
    {
        public bool IsInitialized => false;

        public Task CancelRecordTransfer()
        {
            throw new NotImplementedException();
        }

        public Task Connect(BluetoothDevice device, IBLEGaugePresenter gaugeInformation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAllRecords()
        {
            throw new NotImplementedException();
        }

        public Task DeleteRecord(IDeleteRequest deleteRequest)
        {
            throw new NotImplementedException();
        }

        public Task<GaugeRecord?> GetRecord(ITransferRequest transferRequest, bool withAScans)
        {
            throw new NotImplementedException();
        }

        public Task<List<GaugeRecordSummary>?> GetRecordList()
        {
            throw new NotImplementedException();
        }

        public Task NewRecord(BlankRecord record)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeToLiveUpdates()
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeFromLiveUpdates()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
        }

        public void Dispose()
        {
        }
    }
}