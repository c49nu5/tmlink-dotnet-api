using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface IGauge : IDisposable
    {
        bool IsConnected { get; }
        string Name { get; }
        string Model { get; }
        Version? FirmwareVersion { get; }
        string SerialNumber { get; }
        GaugeType GaugeType { get; }
        string Port { get; }
        int MaxMaterialCount { get; }
        int MinCommentCount { get; }

        void AddObserver(IGaugeObserver observer);

        Task<List<GaugeRecordSummary>?> GetRecordList();
        Task<GaugeRecord?> GetRecord(IFileTransferRequest transferRequest, bool withAScans);
        Task CancelRecordTransfer();
        Task DeleteAllRecords();
        Task DeleteRecord(IFileTransferRequest deleteRequest);
        Task NewRecord(BlankRecord record);

        ErrorCode DoProbeZero();
        GaugeInformation GetConnectionInfo();
        void SendCommentList(string[] commentsList);
        void SendMaterialList(List<Material> materialList);
        ErrorCode SendVelocity(uint velocity, MeasurementUnits units);
        ErrorCode SendMeasurementSetup(IMeasurementSettingsUpdate measurementSettingsUpdate, MeasurementUnits units, MeasurementResolution resolution);
        ErrorCode SendMeasurementSetup(MeasurementUnits units, MeasurementResolution resolution);

        Task SubscribeToLiveUpdates();
        void UnsubscribeFromLiveUpdates();
        void Disconnect();
    }
}