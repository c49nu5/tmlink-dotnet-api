using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.V1;
using Cygnus.Interfaces;
using Cygnus.Models;
using Microsoft.Extensions.Logging;
using static Cygnus.BLE.Protobuf.V1.Constants;

namespace Cygnus.BLE.Protobuf.Services
{
    internal class Protobuf1Channel : ProtobufChannel
    {
        protected IProtobufMessageConverter _protobufMessageConverter;

        public Protobuf1Channel(ILogger<Protobuf1Channel> logger, IProtobufMessageConverter protobufMessageConverter, Protobuf1CommandHandler protobuf1CommandHandler) 
            : this(protobuf1CommandHandler, logger, protobufMessageConverter)
        {
        }

        internal Protobuf1Channel(IProtobufCommandHandler protobuf1CommandHandler, ILogger<Protobuf1Channel> logger, IProtobufMessageConverter protobufMessageConverter)
            : base(logger, protobuf1CommandHandler)
        {
            _protobufMessageConverter = protobufMessageConverter ?? throw new ArgumentNullException(nameof(protobufMessageConverter));
        }
     
        public override async Task<List<GaugeRecordSummary>?> GetRecordList()
        {
            return (await DoGetRecordList()).Union(await DoGetBScanList()).ToList();
        }

        private async Task<List<GaugeRecordSummary>> DoGetBScanList()
        {
            Command command = new()
            {
                commandType = V1.CommandType.GetBScanList,
                Timestamp = DateTime.Now
            };

            var bScanList = await _protobufCommandHandler.SendCommandWithResponse<Message.BScanList, Message>(command, m => m.bscanList);
            _logger.LogInformation("Received bscan list from gauge {Device}: {RecordCount}", _device?.Name, bScanList?.Items.Count);

            return bScanList?.Items.Select(i => new GaugeRecordSummary
            {
                FileSize = i.fileSize,
                Name = i.Name,
                Key = i.Key,
                NumberOfPointsRequired = i.numScanPoints,
                NumberOfPointsTaken = i.numScanPoints,
                RecordType = Models.RecordType.BScan,
                Updated = i.Updated,
            }).ToList() ?? [];
        }

        private async Task<List<GaugeRecordSummary>> DoGetRecordList()
        {
            Command command = new()
            {
                commandType = V1.CommandType.GetRecordList,
                Timestamp = DateTime.Now,                
            };

            var recordList = await _protobufCommandHandler.SendCommandWithResponse<Message.RecordList, Message>(command, m => m.recordList);
            _logger.LogInformation("Received record list from gauge {Device}: {RecordCount}", _device?.Name, recordList?.Items.Count);

            return recordList?.Items.Select(i => new GaugeRecordSummary
            {
                FileSize = i.fileSize,
                Name = i.Name,
                Key = i.Key,
                NumberOfPointsRequired = i.numPointsRequired,
                NumberOfPointsTaken = i.numPointsTaken,
                RecordType = (Models.RecordType)i.recordType,
                Created = i.Created,
                Updated = i.Updated,
            }).ToList() ?? [];
        }

        protected override async Task<GaugeRecord?> GetGaugeRecord(ITransferRequest transferRequest)
        {
            Command command = new()
            {
                commandType = V1.CommandType.GetRecord,
                Name = transferRequest.Name,
                Timestamp = DateTime.Now
            };

            Message.Record? record = await _protobufCommandHandler.SendCommandWithResponse<Message.Record, Message>(command, m => m.record, _recordTransferCts?.Token);

            if (record != null)
            {
                _logger.LogInformation("Received record from gauge {RecordName}: {RequiredPoints} {PointsTaken}", record.Name, record.numPointsRequired, record.numPointsTaken);
                GaugeRecord gaugeRecord = new()
                {
                    Name = record.Name,
                    Key = record.Key,
                    Location = record.Location,
                    RecordID = record.recordID,
                    RecordType = (Models.RecordType)record.recordType,
                    Surveyor = record.Surveyor,
                    Created = record.Created,
                    Updated = record.Updated,
                    NumberPointsRequired = record.numPointsRequired,
                    NumberOfPointsTaken = record.numPointsTaken
                };

                return gaugeRecord;
            }

            _logger.LogWarning("Did not receive record from gauge {RecordName}", transferRequest.Name);
            return null;
        }

        protected override async Task<MeasurementPoint?> GetMeasurementPoint(string recordName, bool withAscans = false)
        {
            Command command = new()
            {
                commandType = withAscans ? V1.CommandType.GetRecordPointAScan : V1.CommandType.GetRecordPoint,
                Name = recordName
            };

            var measurement = await _protobufCommandHandler.SendCommandWithResponse<Message.RecordPoint, Message>(command, m => m.recordPoint, _recordTransferCts?.Token);
            if (measurement != null)
            {
                _logger.LogInformation("Received record point from gauge {RecordId}: {PointName}", measurement.recordID, measurement.Name);
                return new()
                {
                    Name = measurement.Name,
                    RecordID = measurement.recordID,
                    Key = measurement.Key,
                    Method = (Models.Method)measurement.Method,
                    Mode = (Models.UTMode)measurement.UTMode,
                    ProbeType = (Models.ProbeType)measurement.probeType,
                    Timestamp = measurement.Taken,
                    ColNumX = measurement.colNumX,
                    RowNumY = measurement.rowNumY,
                    Units = (MeasurementUnits)measurement.Uom,
                    Thickness = measurement.Thickness,
                    Velocity = measurement.Velocity,
                    AScan = GetAScan(measurement.Ascan)
                };
            }

            _logger.LogWarning("Did not receive record point from gauge {RecordName}", recordName);
            return null;
        }

        protected override async Task<GaugeRecord?> GetGaugeBScan(ITransferRequest transferRequest)
        {
            Command command = new()
            {
                commandType = V1.CommandType.GetBScan,
                Name = transferRequest.Name,
                Timestamp = DateTime.Now
            };

            Message.BScan? bScan = await _protobufCommandHandler.SendCommandWithResponse<Message.BScan, Message>(command, m => m.Bscan, _recordTransferCts?.Token);
            if (bScan != null)
            {
                _logger.LogInformation("Received b-scan from gauge {BScanName}: {PointsTaken}", bScan.Name, bScan.numScanPoints);
                GaugeRecord gaugeRecord = new()
                {
                    Name = bScan.Name,
                    Key = bScan.Key,
                    RecordID = bScan.BScanID,
                    RecordType = Models.RecordType.BScan,
                    Updated = bScan.Updated,
                    NumberPointsRequired = bScan.numScanPoints,
                    NumberOfPointsTaken = bScan.numScanPoints
                };
                return gaugeRecord;
            }

            _logger.LogWarning("Did not receive b-scan from gauge {BScanName}", transferRequest.Name);
            return null;
        }

        protected override async Task<MeasurementPoint?> GetBScanPoint(string recordName, bool withAscans = false)
        {
            Command command = new()
            {
                commandType = withAscans ? V1.CommandType.GetBScanPointAScan : V1.CommandType.GetBScanPoint,
                Name = recordName
            };

            var measurement = await _protobufCommandHandler.SendCommandWithResponse<Message.BScanPoint, Message>(command, m => m.bscanPoint, _recordTransferCts?.Token);
            if (measurement != null)
            {
                _logger.LogInformation("Received record point from gauge {RecordId}", measurement.BScanID);
                return new()
                {
                    Name = measurement.scanPointNum.ToString(),
                    RecordID = measurement.BScanID,
                    Method = Models.Method.Scan,
                    Mode = (Models.UTMode)measurement.UTMode,
                    ProbeType = (Models.ProbeType)measurement.probeType,
                    Units = (MeasurementUnits)measurement.Uom,
                    Thickness = measurement.Thickness,
                    Velocity = measurement.Velocity,
                    AScan = GetAScan(measurement.Ascan)
                };
            }

            _logger.LogWarning("Did not receive b-scan point from gauge {BScanName}", recordName);
            return null;
        }

        private Models.AScan? GetAScan(V1.AScan ascan)
        {
            if (ascan == null)
            {
                return null;
            }

            return new()
            {
                Rectify = (Models.AScanRectify)ascan.Rectify,
                AScanPoints = ascan.ascanPoints.ToArray(),
                AScanStart = ascan.ascanStart,
                AScanWidth = ascan.ascanWidth,
                Echo1 = ascan.Echo1,
                Echo2 = ascan.Echo2,
                Echo3 = ascan.Echo3
            };
        }

        public override async Task DeleteRecord(IDeleteRequest deleteRequest)
        {
            await CancelRecordTransfer();

            Command command = new()
            {
                commandType = deleteRequest.RecordType == Models.RecordType.BScan ? V1.CommandType.DeleteBScan : V1.CommandType.DeleteRecord,
                Name = deleteRequest.Name
            };

            await _protobufCommandHandler.SendCommand(command);
        }

        public override async Task DeleteAllRecords()
        {
            await CancelRecordTransfer();

            Command command = new()
            {
                commandType = V1.CommandType.DeleteAllRecords,
            };

            await _protobufCommandHandler.SendCommand(command);

            command = new()
            {
                commandType = V1.CommandType.DeleteAllBScans,
            };

            await _protobufCommandHandler.SendCommand(command);
        }

        public override async Task NewRecord(BlankRecord record)
        {
            Command.NewRecord newRecord = new()
            {
                Key = record.Key,
                Name = record.Name,
                recordType = (V1.RecordType)record.Type,
                Uom = (Uom)record.Units,
                numColsX = record.Type == Models.RecordType.Grid ? record.ColumnCount : (uint)record.MeasurementPoints.Length,
                numRowsY = record.Type == Models.RecordType.Grid ? record.RowCount : 1,
                gridPattern = (V1.GridPattern)record.GridPattern                
            };

            Command command = new()
            {
                commandType = V1.CommandType.NewRecord,
                newRecord = newRecord,
                Timestamp = DateTime.Now,                
            };

            await _protobufCommandHandler.SendCommand(command);

            const int batchSize = 10;
            for (var i = 0; i < record.MeasurementPoints.Length; i = i + batchSize)
            {
                Command.AddRecordPoints addRecordPoints = new() { Name = record.Name };
                var p = 0;
                foreach (var point in record.MeasurementPoints.Skip(i).Take(batchSize))
                {
                    addRecordPoints.Mpoints.Add(
                        new Command.AddRecordPoints.MPoint
                        {
                            Key = point.Key,
                            Name = point.Name,
                            colNumX = record.Type == Models.RecordType.Linear ? (uint)(i + p) : point.ColNumX,
                            rowNumY = record.Type == Models.RecordType.Linear ? 0u : point.RowNumY,
                            Method = (V1.Method)point.Method,
                            minThickness = point.ThicknessMinLimit,
                            maxThickness = point.ThicknessMaxLimit
                        });
                    p++;
                }

                Command pointsCommand = new()
                {
                    commandType = V1.CommandType.AddRecordPoints,
                    addRecordPoints = addRecordPoints
                };

                await _protobufCommandHandler.SendCommand(pointsCommand);
            }
        }
        
        public override async Task CancelRecordTransfer()
        {
            await base.CancelRecordTransfer();
            Command command = new()
            {
                commandType = V1.CommandType.CancelRecordTransfer,
            };

            await _protobufCommandHandler.SendCommand(command, true);
        }

        protected override void UpdateLiveMeasurement(byte[] value)
        {
            NotifyLiveMeasurement liveMeasurement = _protobufMessageConverter.FromProtobuf<NotifyLiveMeasurement>(value);
            if (liveMeasurement != null)
            {
                if (liveMeasurement.liveMeasurementType == LiveMeasurementType.Frozen)
                {
                    var frozenCharacteristic = _frozenCharacteristic;
                    if (frozenCharacteristic == null)
                    {
                        _logger.LogError("Frozen characteristic not found");
                    }
                    else
                    {
                        frozenCharacteristic.ReadValue().ContinueWith(task =>
                        {
                            if (task.IsCompletedSuccessfully && task.Result.Length > 0)
                            {
                                var frozenMeasurement = _protobufMessageConverter.FromZippedProtobuf<FrozenLiveMeasurement>(task.Result);
                                _logger.LogInformation("Received frozen measurement from gauge {DeviceIdentifier}: {serialNumber}", _device?.Name, frozenMeasurement.Index);
                                NotifyObservers(o => o.OnLiveMeasurementReceived(new LiveMeasurement
                                {
                                    BatteryLevel = frozenMeasurement.batteryLevel,
                                    GaindB = frozenMeasurement.gaindB,
                                    Index = frozenMeasurement.Index,
                                    Units = (MeasurementUnits)frozenMeasurement.Uom,
                                    SurfaceTemp = frozenMeasurement.surfaceTemp,
                                    Thickness = frozenMeasurement.Thickness,
                                    Mode = (Models.UTMode)frozenMeasurement.UTMode,
                                    Velocity = frozenMeasurement.Velocity,
                                    IsDeepcoat = (liveMeasurement.statusBits & DeepcoatFlag) == DeepcoatFlag,
                                    IsFrozen = true,
                                    IsStable = frozenMeasurement.stableMeasurement,
                                    IsValid = frozenMeasurement.validMeasurement,
                                    AScan = GetAScan(frozenMeasurement.Ascan)
                                }));
                            }
                            else
                            {
                                _logger.LogError(task.Exception, "Error retrieving frozen measurement for gauge {DeviceIdentifier}", _device?.Name);
                            }
                        });
                    }
                }
                else
                {
                    NotifyObservers(o => o.OnLiveMeasurementReceived(new LiveMeasurement
                    {
                        BatteryLevel = liveMeasurement.batteryLevel,
                        GaindB = liveMeasurement.gaindB,
                        Index = liveMeasurement.Index,
                        Units = (liveMeasurement.statusBits & IsImperialUnits) == IsImperialUnits ? MeasurementUnits.Imperial : MeasurementUnits.Metric,
                        SurfaceTemp = liveMeasurement.surfaceTemp,
                        Thickness = liveMeasurement.Thickness,
                        Mode = (Models.UTMode)liveMeasurement.UTMode,
                        Velocity = liveMeasurement.Velocity,
                        IsDeepcoat = (liveMeasurement.statusBits & DeepcoatFlag) == DeepcoatFlag,
                        IsFrozen = (liveMeasurement.statusBits & IsFrozenFlag) == IsFrozenFlag,
                        IsStable = (liveMeasurement.statusBits & IsStableFlag) == IsStableFlag,
                        IsValid = (liveMeasurement.statusBits & IsValidFlag) == IsValidFlag,
                    }));
                }
            }
        }

        protected override async Task<GaugeInformation> GetGaugeInformation()
        {
            Command command = new()
            {
                commandType = V1.CommandType.GetGaugeInfo,
                Timestamp = DateTime.Now
            };

            var gaugeInfo = await _protobufCommandHandler.SendCommandWithResponse<Message.GaugeInfo, Message>(command, m => m.gaugeInfo);
            if (gaugeInfo != null)
            {
                _logger.LogInformation("Updated gauge info for {Device}: Serial Number {SerialNumber}", _device?.Name, gaugeInfo.serialNumber);
            }
            else
            {
                _logger.LogError("No gauge info returned for device {Device}", _device?.Name);
            }

            return new GaugeInformation
            {
                SerialNumber = gaugeInfo?.serialNumber ?? 0,
                GaugeUD = gaugeInfo?.gaugeUD ?? 0,
                VersionNumber = gaugeInfo?.versionNumber ?? 0,
                BatteryLevel = gaugeInfo?.batteryLevel ?? 0,
                GaugeVariant = (Models.GaugeVariant)(gaugeInfo?.gaugeVariant ?? V1.GaugeVariant.Test)
            };
        }
    }
}
