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
        public Task Disconnect() => Task.CompletedTask;
        public Task<bool> CancelRecordTransfer() => Task.FromResult(false);

        public Task<bool> Connect(ITMLinkDevice device, ILiveMeasurementObserver gauge) => throw new NotImplementedException();
        public Task DeleteAllRecords() => throw new NotImplementedException();
        public Task DeleteRecord(IFileTransferRequest deleteRequest) => throw new NotImplementedException();
        public Task<GaugeRecord?> GetRecord(IFileTransferRequest transferRequest, bool withAScans) => throw new NotImplementedException();
        public Task<List<GaugeRecordSummary>?> GetRecordList() => throw new NotImplementedException();
        public Task NewRecord(BlankRecord record) => throw new NotImplementedException();
        public void AddObserver(ILiveMeasurementObserver observer) => throw new NotImplementedException();
        public void RemoveObserver(ILiveMeasurementObserver observer) => throw new NotImplementedException();
        public Task<GaugeInformation?> GetGaugeInformation() => throw new NotImplementedException();

        public void Dispose() {}

    }
}