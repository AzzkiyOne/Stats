using System.Collections.Generic;
using Stats.Widgets;
using Stats.Widgets.Table.Filters.Widgets;

namespace Stats.ColumnWorkers;

public abstract class ColumnWorker<ValueType>
    : IColumnWorker
{
    public abstract ColumnCellStyle CellStyle { get; }
#pragma warning disable CS8618
    public ColumnDef ColumnDef { get; set; }
#pragma warning restore CS8618
    private readonly Dictionary<ThingAlike, ValueType> ValuesCache = [];
    protected abstract ValueType GetValue(ThingAlike thing);
    public ValueType GetValueCached(ThingAlike thing)
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
    protected abstract IWidget GetTableCellContent(ValueType value, ThingAlike thing);
    public IWidget? GetTableCellWidget(ThingAlike thing)
    {
        var value = GetValueCached(thing);

        if (ShouldShowValue(value) == false)
        {
            return null;
        }

        return GetTableCellContent(value, thing);
    }
    public abstract IFilterWidget GetFilterWidget();
    public abstract int Compare(ThingAlike thing1, ThingAlike thing2);
}
