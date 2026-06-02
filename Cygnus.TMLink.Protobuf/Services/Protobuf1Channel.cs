using Cygnus.TMLink.Protobuf.Interfaces;
using Cygnus.TMLink.Protobuf.V1;
using Cygnus.Interfaces;
using Cygnus.Models;
using Microsoft.Extensions.Logging;
using static Cygnus.TMLink.Protobuf.V1.Constants;

namespace Cygnus.TMLink.Protobuf.Services
{
    internal class Protobuf1Channel : ProtobufChannel
    {
        protected IProtobufMessageConverter _protobufMessageConverter;

        public Protobuf1Channel(
            ILogger<Protobuf1Channel> logger,
            IProtobufMessageConverter protobufMessageConverter,
            Protobuf1CommandHandler protobuf1CommandHandler) 
            : this(protobuf1CommandHandler, logger, protobufMessageConverter)
        {
        }

        internal Protobuf1Channel(
            IProtobufCommandHandler protobuf1CommandHandler,
            ILogger<Protobuf1Channel> logger,
            IProtobufMessageConverter protobufMessageConverter)
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

            return bScanList?.Items
                .Select(i => new GaugeRecordSummary(
                    default,
                    i.Name,
                    default,
                    Models.RecordType.BScan,
                    i.fileSize,
                    null,
                    i.Updated,
                    i.Key,
                    i.numScanPoints,
                    i.numScanPoints))
                .ToList() ?? [];
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

            return recordList?.Items
                .Select(i => new GaugeRecordSummary(
                    default,
                    i.Name,
                    default,
                    i.recordType == V1.RecordType.Linear ? Models.RecordType.Linear : Models.RecordType.Grid2D,
                    i.fileSize,
                    i.Created,
                    i.Updated,
                    i.Key,
                    i.numPointsRequired,
                    i.numPointsTaken))
                .ToList() ?? [];
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
                    RecordType = ConvertToRecordType(record.recordType),
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

        protected override async Task<Measurement?> GetMeasurementPoint(string recordName, bool withAscans = false)
        {
            Command command = new()
            {
                commandType = withAscans ? V1.CommandType.GetRecordPointAScan : V1.CommandType.GetRecordPoint,
                Name = recordName
            };

            var measurement = await _protobufCommandHandler.SendCommandWithResponse<Message.RecordPoint, Message>(command, m => m.recordPoint, _recordTransferCts?.Token);
            if (measurement != null && measurement.State != MeasurementPointState.Deleted)
            {
                _logger.LogInformation("Received record point from gauge {RecordId}: {PointName}", measurement.recordID, measurement.Name);
                return new()
                {
                    Name = measurement.Name,
                    Type = ConvertToMeasurementType(measurement.State),
                    RecordId = measurement.recordID,
                    Key = measurement.Key,
                    Method = (Models.Method)measurement.Method,
                    Mode = ConvertToMeasureMode(measurement.UTMode),
                    //Probe = (Models.ProbeType)measurement.probeType,                   
                    Time = measurement.Taken,
                    GridCoordinate = new() { Column = (ushort)measurement.colNumX, Row = (ushort)measurement.rowNumY },
                    Units = (MeasurementUnits)measurement.Uom,
                    Thickness = measurement.Thickness,
                    Velocity = measurement.Velocity,
                    AScan = GetAScan(measurement.Ascan),
                    EchoPoints = [.. GetEchoPoints(measurement.Ascan)],
                    HasAScan = measurement.Ascan?.ascanPoints?.Length > 0
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

        protected override async Task<Measurement?> GetBScanPoint(string recordName, bool withAscans = false)
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
                    Type = MeasurementType.Valid,
                    RecordId = measurement.BScanID,
                    Method = Models.Method.Scan,
                    Mode = ConvertToMeasureMode(measurement.UTMode),
                    //Probe = (Models.ProbeType)measurement.probeType,
                    Units = (MeasurementUnits)measurement.Uom,
                    Thickness = measurement.Thickness,
                    Velocity = measurement.Velocity,
                    AScan = GetAScan(measurement.Ascan),
                    EchoPoints = [.. GetEchoPoints(measurement.Ascan)],
                    HasAScan = measurement.Ascan?.ascanPoints?.Length > 0
                };
            }

            _logger.LogWarning("Did not receive b-scan point from gauge {BScanName}", recordName);
            return null;
        }

        private Models.AScan GetAScan(V1.AScan? ascan)
        {
            if (ascan == null)
            {
                return new Models.AScan();
            }

            return new()
            {
                RectifyMode = (RectifyMode)ascan.Rectify,
                Amplitudes = [.. ascan.ascanPoints.Select(p => (sbyte)p)],
                StartTime = ascan.ascanStart,
                WidthTime = ascan.ascanWidth
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
            if (record.Type != Models.RecordType.Linear && record.Type != Models.RecordType.Grid2D)
            {
                throw new ArgumentException($"Unsupported record type {record.Type} for new record command, can only create linear or grid records");
            }

            Command.NewRecord newRecord = new()
            {
                Key = record.Key,
                Name = record.Name,
                recordType = ConvertToRecordType(record.Type),
                Uom = (Uom)record.Units,
                numColsX = record.Type == Models.RecordType.Grid2D ? record.ColumnCount : (uint)record.MeasurementPoints.Length,
                numRowsY = record.Type == Models.RecordType.Grid2D ? record.RowCount : 1,
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
                                    SurfaceTemperatureCelsius = (int)frozenMeasurement.surfaceTemp,
                                    Thickness = frozenMeasurement.Thickness,
                                    Mode = ConvertToMeasureMode(frozenMeasurement.UTMode),
                                    Velocity = frozenMeasurement.Velocity,
                                    DeepCoatOn = (liveMeasurement.statusBits & DeepcoatFlag) == DeepcoatFlag,
                                    HasAScan = frozenMeasurement.Ascan?.ascanPoints?.Length > 0,
                                    IsFrozen = true,
                                    StableMeasurement = frozenMeasurement.stableMeasurement,
                                    ValidMeasurement = frozenMeasurement.validMeasurement,
                                    AScan = GetAScan(frozenMeasurement.Ascan),
                                    EchoPoints = [.. GetEchoPoints(frozenMeasurement.Ascan)],
                                    Type = frozenMeasurement.Thickness > 0 ? MeasurementType.Valid : MeasurementType.None,
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
                        SurfaceTemperatureCelsius = (int)liveMeasurement.surfaceTemp,
                        Thickness = liveMeasurement.Thickness,
                        Mode = ConvertToMeasureMode(liveMeasurement.UTMode),
                        Velocity = liveMeasurement.Velocity,
                        DeepCoatOn = (liveMeasurement.statusBits & DeepcoatFlag) == DeepcoatFlag,
                        IsFrozen = (liveMeasurement.statusBits & IsFrozenFlag) == IsFrozenFlag,
                        StableMeasurement = (liveMeasurement.statusBits & IsStableFlag) == IsStableFlag,
                        ValidMeasurement = (liveMeasurement.statusBits & IsValidFlag) == IsValidFlag,
                        Type = liveMeasurement.Thickness > 0 ? MeasurementType.Valid : MeasurementType.None,
                    }));
                }
            }
        }

        private IEnumerable<EchoPoint> GetEchoPoints(V1.AScan? ascan)
        {
            if (ascan == null)
            {
                yield break;
            }

            if (ascan.Echo1 > 0)
            {
                yield return new EchoPoint { Time = ascan.Echo1 };
            }

            if (ascan.Echo2 > 0)
            {
                yield return new EchoPoint { Time = ascan.Echo2 };
            }

            if (ascan.Echo3 > 0)
            {
                yield return new EchoPoint { Time = ascan.Echo3 };
            }
        }

        private MeasureMode ConvertToMeasureMode(UTMode uTMode)
        {
            return uTMode switch
            {
                UTMode.Se => MeasureMode.SingleEcho,
                UTMode.Ee => MeasureMode.EchoEcho,
                UTMode.Me => MeasureMode.MultipleEcho,
                UTMode.Gt => MeasureMode.Manual,
                _ => MeasureMode.Auto,
            };
        }

        private Models.RecordType ConvertToRecordType(V1.RecordType recordType)
        {
            return recordType switch
            {
                V1.RecordType.Linear => Models.RecordType.Linear,
                V1.RecordType.Grid => Models.RecordType.Grid2D,
                _ => Models.RecordType.None,
            };
        }

        private V1.RecordType ConvertToRecordType(Models.RecordType type)
        {
            return type switch
            {
                Models.RecordType.Grid2D => V1.RecordType.Grid,
                _ => V1.RecordType.Linear,
            };
        }

        private MeasurementType ConvertToMeasurementType(MeasurementPointState state)
        {
            return state switch
            {
                MeasurementPointState.Valid => MeasurementType.Valid,
                MeasurementPointState.Obstructed => MeasurementType.Obstructed,
                MeasurementPointState.NoReading => MeasurementType.NoReading,
                _ => MeasurementType.None,
            };
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
                GaugeId = gaugeInfo?.gaugeUD ?? 0,
                SoftwareVersionNumber = gaugeInfo?.versionNumber ?? 0,
                BatteryLevel = gaugeInfo?.batteryLevel ?? 0,
                GaugeVariant = (Models.GaugeVariant)(gaugeInfo?.gaugeVariant ?? V1.GaugeVariant.Test)
            };
        }
    }
}
