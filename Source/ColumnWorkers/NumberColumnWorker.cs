using System;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public sealed class NumberColumnWorker<T> : ColumnWorker<T>
{
    private readonly string UnitOfMeasure;
    private readonly Func<T, decimal> GetValue;
    public NumberColumnWorker(Func<T, decimal> valueFunction, string unitOfMeasure = "")
        : base(TableColumnCellStyle.Number)
    {
        GetValue = valueFunction.Memoized();
        UnitOfMeasure = unitOfMeasure;
    }
    public sealed override Widget? GetTableCellWidget(T thing)
    {
        var value = GetValue(thing);

        if (value == 0m)
        {
            return null;
        }

        return new Label(value.ToString() + UnitOfMeasure);
    }
    public sealed override FilterWidget<T> GetFilterWidget()
    {
        return new NumberFilter<T>(GetValue);
    }
    public sealed override int Compare(T thing1, T thing2)
    {
        return GetValue(thing1).CompareTo(GetValue(thing2));
    }
}
