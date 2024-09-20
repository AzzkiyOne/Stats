namespace Stats.Table.Columns;

public abstract class Column_Num : Column
{
    protected abstract float? GetValue(ThingAlike thing);
    protected virtual string FormatValue(float value)
    {
        return value.ToString();
    }
    public override ICell? GetCell(ThingAlike thing)
    {
        return GetValue(thing) is float value
            ? new Cells.Cell<float>(value, FormatValue(value))
            : null;
    }
    //public virtual IFilter GetFilter()
    //{
    //    return new Filter_Num(this, GetValue);
    //}
}
