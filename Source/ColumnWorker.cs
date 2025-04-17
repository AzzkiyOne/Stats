namespace Stats;

public abstract class ColumnWorker<ValueType>
    : IColumnWorker<ValueType>
{
    public abstract ColumnCellStyle CellStyle { get; }
#pragma warning disable CS8618
    public ColumnDef ColumnDef { get; set; }
#pragma warning restore CS8618
    public abstract ValueType GetValue(ThingRec thing);
    protected virtual bool ShouldShowValue(ValueType value)
    {
        return value != null;
    }
    protected abstract IWidget GetTableCellContent(ValueType value, ThingRec thing);
    public IWidget? GetTableCellContent(ThingRec thing)
    {
        var value = GetValue(thing);

        if (ShouldShowValue(value) == false)
        {
            return null;
        }

        return GetTableCellContent(value, thing);
    }
    public abstract IWidget_FilterInput GetFilterWidget();
    public abstract int Compare(ThingRec thing1, ThingRec thing2);
}
