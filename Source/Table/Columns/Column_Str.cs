namespace Stats.Table.Columns;

public abstract class Column_Str : Column
{
    protected abstract string? GetValue(ThingAlike thing);
    public override ICell? GetCell(ThingAlike thing)
    {
        return GetValue(thing) is { Length: > 0 } value
            ? new Cells.Cell_Str(value)
            : null;
    }
}
