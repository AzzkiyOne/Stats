using System.Collections.Generic;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public abstract class ColumnWorker
{
#pragma warning disable CS8618
    public ColumnDef ColumnDef { get; set; }
#pragma warning restore CS8618
    public abstract TableColumnCellStyle CellStyle { get; }
    public abstract Widget? GetTableCellWidget(ThingAlike thing);
    public abstract FilterWidget GetFilterWidget();
    public abstract int Compare(ThingAlike thing1, ThingAlike thing2);
}

public abstract class ColumnWorker<T> : ColumnWorker
{
    private readonly Dictionary<ThingAlike, T> ValuesCache = [];
    protected abstract T GetValue(ThingAlike thing);
    public T GetValueCached(ThingAlike thing)
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
    protected virtual bool ShouldShowValue(T value)
    {
        return value != null;
    }
    protected abstract Widget GetTableCellContent(T value, ThingAlike thing);
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var value = GetValueCached(thing);

        if (ShouldShowValue(value) == false)
        {
            return null;
        }

        return GetTableCellContent(value, thing);
    }
}
