using System;
using Stats.Widgets;

namespace Stats;

public sealed class NumberColumnWorker<TObject> : ColumnWorker<TObject>
{
    private readonly string UnitOfMeasure;
    private readonly Func<TObject, decimal> GetValue;
    public NumberColumnWorker(Func<TObject, decimal> valueFunction, string unitOfMeasure = "")
        : base(TableColumnCellStyle.Number)
    {
        GetValue = valueFunction;
        UnitOfMeasure = unitOfMeasure;
    }
    public sealed override Widget? GetTableCellWidget(TObject @object)
    {
        var value = GetValue(@object);

        if (value == 0m)
        {
            return null;
        }

        return new Label(value.ToString() + UnitOfMeasure);
    }
    public sealed override FilterWidget<TObject> GetFilterWidget()
    {
        return new NumberFilter<TObject>(GetValue);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return GetValue(object1).CompareTo(GetValue(object2));
    }
}
