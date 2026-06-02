using Cygnus.Models;
using Cygnus.TMLink.Protobuf.Interfaces;
using Cygnus.Interfaces;
using Cygnus.TMLink.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Cygnus.TMLink.Protobuf.Services
{
    [ExcludeFromCodeCoverage]
    public class ProtobufNullChannel : IProtobufChannel
    {
        public bool IsInitialized => false;

        public Task CancelRecordTransfer()
        {
            throw new NotImplementedException();
        }

        public Task<GaugeInformation?> Connect(ITMLinkDevice device)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAllRecords()
        {
            throw new NotImplementedException();
        }

        public Task DeleteRecord(IFileTransferRequest deleteRequest)
        {
            throw new NotImplementedException();
        }

        public Task<GaugeRecord?> GetRecord(IFileTransferRequest transferRequest, bool withAScans)
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