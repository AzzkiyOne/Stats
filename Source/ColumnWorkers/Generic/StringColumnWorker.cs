using Stats.Widgets;
using Stats.Widgets.Misc;
using Stats.Widgets.Table.Filters.Widgets;

namespace Stats.ColumnWorkers.Generic;

public abstract class StringColumnWorker
    : ColumnWorker<string?>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.String;
    protected override bool ShouldShowValue(string? value)
    {
        return value?.Length > 0;
    }
    protected override IWidget GetTableCellContent(string? value, ThingAlike thing)
    {
        return new Label(value);
    }
    public override IFilterWidget GetFilterWidget()
    {
        return new StringFilterWidget(GetValueCached);
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
