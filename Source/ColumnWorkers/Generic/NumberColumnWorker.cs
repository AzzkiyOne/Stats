using System;
using System.Globalization;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers.Generic;

public abstract class NumberColumnWorker<T>
    : ColumnWorker<T>
    where T :
        struct,
        IEquatable<T>,
        IComparable<T>,
        IFormattable
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.Number;
    private
    protected virtual string FormatValue(T value)
    {
        if (ColumnDef.formatString != null)
        {
            return value.ToString(
                ColumnDef.formatString,
                CultureInfo.CurrentCulture.NumberFormat
            );
        }

        return value.ToString();
    }
    protected override bool ShouldShowValue(T value)
    {
        return value.Equals(default) == false;
    }
    protected override IWidget GetTableCellContent(T value, ThingAlike thing)
    {
        var valueStr = FormatValue(value);

        return new Label(valueStr);
    }
    public override IFilterWidget GetFilterWidget()
    {
        return new NumberFilter<T>(GetValueCached);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).CompareTo(GetValueCached(thing2));
    }
}
