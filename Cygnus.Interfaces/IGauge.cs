using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface IGauge : IDisposable
    {
        bool IsConnected { get; }
        string Name { get; }
        string Model { get; }
        Version? FirmwareVersion { get; }
        uint SerialNumber { get; }
        GaugeType GaugeType { get; }
        string Port { get; }
        GaugeFeatures SupportedFeatures { get; }
        ProbeType ProbeType { get; }

        uint GaugeId { get; }
        uint StatusMessageCount { get; }
        uint BatteryLevel { get; }
        DateTime? GaugeTime { get; }
        GaugeVariant? GaugeVariant { get; }
        uint SoftwareVersionNumber { get; }

        bool IsDataTransferInProgress { get; }

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