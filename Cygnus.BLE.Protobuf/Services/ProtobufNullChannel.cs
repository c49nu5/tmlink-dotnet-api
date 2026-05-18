using Cygnus.Models;
using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.Interfaces;
using Cygnus.BLE.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Cygnus.BLE.Protobuf.Services
{
    [ExcludeFromCodeCoverage]
    public class ProtobufNullChannel : IProtobufChannel
    {
        public bool IsInitialized => false;

        public Task CancelRecordTransfer()
        {
            throw new NotImplementedException();
        }

        public Task<GaugeInformation?> Connect(IBLEDevice device)
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

        public void AddObserver(ILiveMeasurementObserver observer)
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