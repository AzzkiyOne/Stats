using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers.Generic;

public abstract class BooleanColumnWorker
    : ColumnWorker<bool>
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.Boolean;
    protected override bool ShouldShowValue(bool value)
    {
        return value == true;
    }
    protected override Widget GetTableCellContent(bool value, ThingAlike thing)
    {
        var tex = Verse.Widgets.GetCheckboxTexture(value);
        Widget icon = new Icon(tex)
            .SizeAbs(Text.LineHeight)
            .PaddingRel(0.5f, 0f);

        return new SingleElementContainer(icon);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new BooleanFilter(GetValueCached);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).CompareTo(GetValueCached(thing2));
    }
}
