using Cygnus.BLE.Protobuf.Interfaces;
using Cygnus.BLE.Protobuf.V1;
using Cygnus.Interfaces;
using Cygnus.Models;
using Microsoft.Extensions.Logging;
using static Cygnus.BLE.Protobuf.V1.Constants;

namespace Cygnus.BLE.Protobuf.Services
{
    internal class Protobuf1Channel : ProtobufChannel<NotifyMessage>
    {
        public Protobuf1Channel(ILogger<Protobuf1Channel> logger, IProtobufMessageConverter protobufMessageConverter) 
            : base(logger, protobufMessageConverter)
        {
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

            Message.BScanList getRecordList(Message message)
            {
                var recordList = message.bscanList;
                _logger.LogInformation("Received bscan list from gauge {Device}: {RecordCount}", _device?.Name, recordList.Items.Count);
                foreach (var record in recordList.Items)
                {
                    _logger.LogInformation("Record {Index}: {Points}", record.Name, record.numScanPoints);
                }

                return recordList;
            }

            var recordList = await SendCommandWithResponse<Message.BScanList, Message>(command, getRecordList);

            return recordList?.Items.Select(i => new GaugeRecordSummary
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

            Message.RecordList getRecordList(Message message)
            {
                var recordList = message.recordList;
                _logger.LogInformation("Received record list from gauge {Device}: {RecordCount}", _device?.Name, recordList.Items.Count);
                foreach (var record in recordList.Items)
                {
                    _logger.LogInformation("Record {Index}: {Points}", record.Name, record.numPointsRequired);
                }

                return recordList;
            }

            var recordList = await SendCommandWithResponse<Message.RecordList, Message>(command, getRecordList);

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

            Message.Record getRecord(Message message)
            {
                var record = message.record;
                _logger.LogInformation("Received record from gauge {RecordName}: {RequiredPoints} {PointsTaken}", record.Name, record.numPointsRequired, record.numPointsTaken);

                return record;
            }

            Message.Record? record = await SendCommandWithResponse<Message.Record, Message>(command, getRecord);
            if (record != null)
            {
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

            return null;
        }

        protected override async Task<MeasurementPoint?> GetMeasurementPoint(string recordName, bool withAscans = false)
        {
            Command command = new()
            {
                commandType = withAscans ? V1.CommandType.GetRecordPointAScan : V1.CommandType.GetRecordPoint,
                Name = recordName
            };

            Message.RecordPoint getPoint(Message message)
            {
                var point = message.recordPoint;
                _logger.LogInformation("Received record point from gauge {RecordId}: {PointName}", point.recordID, point.Name);

                return point;
            }

            var measurement = await SendCommandWithResponse<Message.RecordPoint, Message>(command, getPoint);
            if (measurement != null)
            {
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

            Message.BScan getRecord(Message message)
            {
                var record = message.Bscan;
                _logger.LogInformation("Received record from gauge {RecordName}: {PointsTaken}", record.Name, record.numScanPoints);

                return record;
            }

            Message.BScan? record = await SendCommandWithResponse<Message.BScan, Message>(command, getRecord);
            if (record != null)
            {
                GaugeRecord gaugeRecord = new()
                {
                    Name = record.Name,
                    Key = record.Key,
                    RecordID = record.BScanID,
                    RecordType = Models.RecordType.BScan,
                    Updated = record.Updated,
                    NumberPointsRequired = record.numScanPoints,
                    NumberOfPointsTaken = record.numScanPoints
                };
                return gaugeRecord;
            }

            return null;
        }

        protected override async Task<MeasurementPoint?> GetBScanPoint(string recordName, bool withAscans = false)
        {
            Command command = new()
            {
                commandType = withAscans ? V1.CommandType.GetBScanPointAScan : V1.CommandType.GetBScanPoint,
                Name = recordName
            };

            Message.BScanPoint getPoint(Message message)
            {
                var point = message.bscanPoint;
                _logger.LogInformation("Received record point from gauge {RecordId}", point.BScanID);

                return point;
            }

            var measurement = await SendCommandWithResponse<Message.BScanPoint, Message>(command, getPoint);
            if (measurement != null)
            {
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

            await SendCommand(command);
        }

        public override async Task DeleteAllRecords()
        {
            await CancelRecordTransfer();

            Command command = new()
            {
                commandType = V1.CommandType.DeleteAllRecords,
            };

            await SendCommand(command);

            command = new()
            {
                commandType = V1.CommandType.DeleteAllBScans,
            };

            await SendCommand(command);
        }

        public override async Task NewRecord(BlankRecord record)
        {
            Command.NewRecord newRecord = new()
            {
                Key = record.Key,
                Name = record.Name,
                recordType = (V1.RecordType)record.Type,
                Uom = (Uom)record.Units,
                numColsX = record.Type == Models.RecordType.Linear ? (uint)record.MeasurementPoints.Length : record.ColumnCount,
                numRowsY = record.Type == Models.RecordType.Linear ? 1 : record.RowCount,
                gridPattern = GridPattern.Dldl                
            };

            Command command = new()
            {
                commandType = V1.CommandType.NewRecord,
                newRecord = newRecord,
                Timestamp = DateTime.Now,                
            };

            await SendCommand(command);

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

                await SendCommand(pointsCommand);
            }
        }
        
        public override async Task CancelRecordTransfer()
        {
            await base.CancelRecordTransfer();
            Command command = new()
            {
                commandType = V1.CommandType.CancelRecordTransfer,
            };

            await SendCommand(command, true);
        }

        protected override void UpdateLiveMeasurement(byte[] value)
        {
            NotifyLiveMeasurement liveMeasurement = _protobufMessageConverter.FromProtobuf<NotifyLiveMeasurement>(value);
            if (liveMeasurement != null)
            {
                NotifyObservers(o => o.UpdateLiveMeasurement(new LiveMeasurement
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

                if (liveMeasurement.liveMeasurementType == LiveMeasurementType.Frozen)
                {
                    FrozenLiveMeasurement getFrozenLiveMeasurement(FrozenLiveMeasurement frozenMeasurement)
                    {
                        _logger.LogInformation("Received frozen measurement from gauge {DeviceIdentifier}: {serialNumber}", _device?.Name, frozenMeasurement.Index);
                        NotifyObservers(o => o.UpdateLiveMeasurement(new LiveMeasurement
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

                        return frozenMeasurement;
                    }

                    GetResponse<FrozenLiveMeasurement, FrozenLiveMeasurement>(BLE.Interfaces.Constants.TMLinkFrozenCharacteristicId, getFrozenLiveMeasurement).ConfigureAwait(false);
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

            Message.GaugeInfo getGaugeInfo(Message message)
            {
                var gaugeInfo = message.gaugeInfo;
                _logger.LogInformation("Received information from gauge {Device}: {serialNumber}", _device?.Name, gaugeInfo.serialNumber);

                return gaugeInfo;
            }

            var gaugeInfo = await SendCommandWithResponse<Message.GaugeInfo, Message>(command, getGaugeInfo);
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
