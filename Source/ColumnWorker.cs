using System.Collections.Generic;

namespace Stats;

public abstract class ColumnWorker<ValueType>
    : IColumnWorker
{
    public abstract ColumnCellStyle CellStyle { get; }
#pragma warning disable CS8618
    public ColumnDef ColumnDef { get; set; }
#pragma warning restore CS8618
    private readonly Dictionary<ThingRec, ValueType> ValuesCache = [];
    protected abstract ValueType GetValue(ThingRec thing);
    public ValueType GetValueCached(ThingRec thing)
    {
        var wasValueCached = ValuesCache.TryGetValue(thing, out var cachedValue);

        if (wasValueCached == false)
        {
            ValuesCache[thing] = cachedValue = GetValue(thing);
        }

        return cachedValue;
    }
    protected virtual bool ShouldShowValue(ValueType value)
    {
        return value != null;
    }
    protected abstract IWidget GetTableCellContent(ValueType value, ThingRec thing);
    public IWidget? GetTableCellWidget(ThingRec thing)
    {
        var value = GetValueCached(thing);

        if (ShouldShowValue(value) == false)
        {
            return null;
        }

        return GetTableCellContent(value, thing);
    }
    public abstract IWidget_FilterInput GetFilterWidget();
    public abstract int Compare(ThingRec thing1, ThingRec thing2);
}
