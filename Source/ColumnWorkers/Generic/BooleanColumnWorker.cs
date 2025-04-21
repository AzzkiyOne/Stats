using Stats.Widgets;
using Stats.Widgets.Comps.Size;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
using Stats.Widgets.Misc;
using Stats.Widgets.Table.Filters.Widgets;
using Verse;

namespace Stats.ColumnWorkers.Generic;

public abstract class BooleanColumnWorker
    : ColumnWorker<bool>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.Boolean;
    protected override bool ShouldShowValue(bool value)
    {
        return value == true;
    }
    protected override IWidget GetTableCellContent(bool value, ThingAlike thing)
    {
        var tex = Verse.Widgets.GetCheckboxTexture(value);
        IWidget icon = new Icon(tex);
        new SetSizeToAbs(ref icon, Text.LineHeight);
        new IncreaseSizeByRel(ref icon, 0.5f, 0f);

        return new SingleElementContainer(icon);
    }
    public override IFilterWidget GetFilterWidget()
    {
        return new BooleanFilterWidget(GetValueCached);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).CompareTo(GetValueCached(thing2));
    }
}
