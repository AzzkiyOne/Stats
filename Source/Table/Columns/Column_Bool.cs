namespace Stats.Table.Columns;

public abstract class Column_Bool : Column
{
    public abstract bool? GetValue(ThingAlike thing);
    public override ICell? GetCell(ThingAlike thing)
    {
        return GetValue(thing) is bool value
            ? new Cells.Cell_Bool(value)
            : null;
    }
    //public virtual IFilter<ThingAlike> GetFilter()
    //{
    //    return new Filter_Bool<ThingAlike>(this, GetValue);
    //}
}
