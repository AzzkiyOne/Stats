using System.Collections.Generic;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public abstract class ColumnWorker<ValueType>
    : IColumnWorker
{
    public abstract TableColumnCellStyle CellStyle { get; }
#pragma warning disable CS8618
    public ColumnDef ColumnDef { get; set; }
#pragma warning restore CS8618
    private readonly Dictionary<ThingAlike, ValueType> ValuesCache = [];
    protected abstract ValueType GetValue(ThingAlike thing);
    public ValueType GetValueCached(ThingAlike thing)
    {
        var wasValueCached = ValuesCache.TryGetValue(
            thing,
            out var cachedValue
        );

        if (wasValueCached == false)
        {
            try
            {
                ValuesCache[thing] = cachedValue = GetValue(thing);
            }
            catch
            {
                // In an event of exception, nothing will be written into the
                // cache. This will cause GetValue to be called again, next time
                // GetValueCached is called. Writing default into the cache is
                // questionable, but better than nothing.
                //
                // TODO: re-evaluate generic type constraint choise.
                ValuesCache[thing] = cachedValue = default;
            }
        }

        return cachedValue;
    }
    protected virtual bool ShouldShowValue(ValueType value)
    {
        return value != null;
    }
    protected abstract IWidget GetTableCellContent(
        ValueType value,
        ThingAlike thing
    );
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
