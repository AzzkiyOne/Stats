namespace Stats.Table.Columns;

public abstract class Column_Str : Column
{
    protected abstract string? GetValue(ThingAlike thing);
    public override ICell? GetCell(ThingAlike thing)
    {
        return GetValue(thing) is { Length: > 0 } value
            ? new Cells.Cell<string>(value)
            : null;
    }
    //public virtual IFilter GetFilter()
    //{
    //    return new Filter_Str(this, GetValue);
    //}
}
