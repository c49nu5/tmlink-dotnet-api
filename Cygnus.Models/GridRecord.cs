using MessagePack;

namespace Cygnus.Models;

[MessagePackObject]
public record GridRecord : Record
{
    [Key(20)]
    public GridType GridType;
    [Key(21)]
    public int RowCount;
    [Key(22)]
    public string RowNamePrefix = string.Empty;
    [Key(23)]
    public int ColumnCount;
    [Key(24)]
    public string ColumnNamePrefix = string.Empty;
    [Key(25)]
    public IEnumerable<string> ColumnNameList = [];

    public void GenerateMeasurements()
    {
        Measurements = [.. CreateGridMeasurements()];
    }

    private IEnumerable<Measurement> CreateGridMeasurements()
    {
        for (int index = 0; index < ColumnCount * RowCount; index++)
        {
            yield return CreateGridMeasurement(index);
        }
    }

    private Measurement CreateGridMeasurement(int index)
    {
        int row = GetRow(index);
        int column = GetColumn(index);
        return new Measurement
        {
            Id = Guid.NewGuid(),
            PointIndex = index,
            Name = $"{RowNamePrefix}{row + 1}.{ColumnNamePrefix}{column + 1}",
            GridCoordinate = new GridCoordinate { Row = (ushort)row, Column = (ushort)column },
            Units = Units,
            Source = MeasurementSource.CygLink
        };
    }

    private int GetRow(int index)
    {
        int row = index / ColumnCount;
        if (GridType.IsHorizontal())
        {
            if (GridType.IsSecondAxisGoingForward())
            {
                return row;
            }

            return RowCount - row - 1;
        }

        row = index % RowCount;
        int column = index / RowCount;
        if (GridType.IsFirstAxisGoingForward(column % 2 == 0))
        {
            return row;
        }

        return RowCount - row - 1;
    }

    private int GetColumn(int index)
    {
        int column = index % ColumnCount;
        if (GridType.IsHorizontal())
        {
            int row = index / ColumnCount;
            if (GridType.IsFirstAxisGoingForward(row % 2 == 0))
            {
                return column;
            }

            return ColumnCount - column - 1;
        }

        column = index / RowCount;
        if (GridType.IsSecondAxisGoingForward())
        {
            return column;
        }

        return ColumnCount - column - 1;
    }
}
