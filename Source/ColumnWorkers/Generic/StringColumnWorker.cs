using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers.Generic;

public abstract class StringColumnWorker
    : ColumnWorker<string?>
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    protected override bool ShouldShowValue(string? value)
    {
        return value?.Length > 0;
    }
    protected override Widget GetTableCellContent(string? value, ThingAlike thing)
    {
        return new Label(value);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new StringFilter(GetValueCached);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        var v1 = GetValueCached(thing1);
        var v2 = GetValueCached(thing2);

        if (v1 == null)
        {
            if (v2 == null)
            {
                return 0;
            }

            return -1;
        }

        return v1.CompareTo(v2);
    }
}
