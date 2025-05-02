using System;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public abstract class NumberColumnWorker : ColumnWorker
{
    public sealed override TableColumnCellStyle CellStyle => TableColumnCellStyle.Number;
    private readonly string UnitOfMeasure;
    private readonly Func<ThingAlike, decimal> GetValue;
    protected NumberColumnWorker(
        Func<ThingAlike, decimal> valueFunction,
        string unitOfMeasure = ""
    )
    {
        UnitOfMeasure = unitOfMeasure;
        GetValue = valueFunction.Memoized();
    }
    public sealed override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var value = GetValue(thing);

        if (value == 0m)
        {
            return null;
        }

        return new Label(value.ToString() + UnitOfMeasure);
    }
    public sealed override FilterWidget GetFilterWidget()
    {
        return new NumberFilter(GetValue);
    }
    public sealed override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValue(thing1).CompareTo(GetValue(thing2));
    }
}
