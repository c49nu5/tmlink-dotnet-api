using Cygnus.Models;

namespace Cygnus.Interfaces
{
    public interface IGauge : IConnectionInformation, IDisposable
    {
        bool IsConnected { get; }
        string Model { get; }
        Version? FirmwareVersion { get; }
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

        // Shared methods for all gauge types

        /// <summary>
        /// Adds an observer to the gauge to receive updates on property changes.
        /// </summary>
        /// <param name="observer"></param>
        void AddObserver(IGaugeObserver observer);

        Task<List<GaugeRecordSummary>?> GetRecordList();
        Task<GaugeRecord?> GetRecord(IFileTransferRequest transferRequest, bool withAScans);
        Task CancelRecordTransfer();
        Task DeleteAllRecords();
        Task DeleteRecord(IFileTransferRequest deleteRequest);

        void SubscribeToLiveUpdates(ILiveMeasurementObserver liveMeasurementObserver);
        void UnsubscribeFromLiveUpdates(ILiveMeasurementObserver liveMeasurementObserver);

        Task Disconnect();

        // TM-Link specific methods
        Task NewRecord(BlankRecord record);

        // Cyglink specific methods
        ErrorCode DoProbeZero();
        void SendCommentList(string[] commentsList);
        void SendMaterialList(List<Material> materialList);
        ErrorCode SendVelocity(uint velocity, MeasurementUnits units);
        ErrorCode SendMeasurementSetup(IMeasurementSettingsUpdate measurementSettingsUpdate, MeasurementUnits units, MeasurementResolution resolution);
        ErrorCode SendMeasurementSetup(MeasurementUnits units, MeasurementResolution resolution);
        ErrorCode SetGaugeTime(DateTime gaugeTime);
    }
}