namespace Cygnus.Models;

public record struct GaugeRecordSummary(int Id, string RecordName, int Directory, RecordType RecordType, uint FileSize, DateTime? Created, DateTime? Updated, string Key, uint NumberOfPointsRequired, uint PointCount) {}
