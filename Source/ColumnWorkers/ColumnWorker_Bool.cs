using Stats.Widgets;
using Stats.Widgets.Comps.Size;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
using Stats.Widgets.Misc;
using Stats.Widgets.Table.Filters;
using Stats.Widgets.Table.Filters.Widgets;
using Verse;

namespace Stats;

public abstract class ColumnWorker_Bool
    : ColumnWorker<bool>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.Boolean;
    protected override bool ShouldShowValue(bool value)
    {
        return value == true;
    }
    protected override IWidget GetTableCellContent(bool value, ThingRec thing)
    {
        var tex = Verse.Widgets.GetCheckboxTexture(value);
        IWidget icon = new Widget_Icon(tex);
        new WidgetComp_Size_Abs(ref icon, Text.LineHeight);
        new WidgetComp_Size_Inc_Rel(ref icon, 0.5f, 0f);

        return new Widget_Container_Single(icon);
    }
    public override IWidget_FilterInput GetFilterWidget()
    {
        return new Widget_FilterInput_Bool(GetValueCached);
    }
    public override int Compare(ThingRec thing1, ThingRec thing2)
    {
        return GetValueCached(thing1).CompareTo(GetValueCached(thing2));
    }
}
