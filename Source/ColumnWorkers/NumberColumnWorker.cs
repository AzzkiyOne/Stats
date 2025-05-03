using System;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public sealed class NumberColumnWorker : ColumnWorker
{
    private readonly string UnitOfMeasure;
    private readonly Func<ThingAlike, decimal> GetValue;
    public NumberColumnWorker(Func<ThingAlike, decimal> valueFunction, string unitOfMeasure = "")
        : base(TableColumnCellStyle.Number)
    {
        GetValue = valueFunction.Memoized();
        UnitOfMeasure = unitOfMeasure;
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
