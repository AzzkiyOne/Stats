using System.Linq;
using RimWorld;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers;

public sealed class StuffCategoryColumnWorker : ColumnWorker<StuffCategoryDef>
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    protected override StuffCategoryDef GetValue(ThingAlike thing)
    {
        return thing.Def.stuffProps?.categories.FirstOrDefault();
    }
    protected override Widget GetTableCellContent(StuffCategoryDef stuffCatDef, ThingAlike thing)
    {
        return StuffCatDefToWidget(stuffCatDef);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new EnumerableFilter<StuffCategoryDef>(
            GetValueCached,
            DefDatabase<StuffCategoryDef>.AllDefsListForReading.OrderBy(stuffCatDef => stuffCatDef.label),
            StuffCatDefToWidget
        );
    }
    private static Widget StuffCatDefToWidget(StuffCategoryDef stuffCatDef)
    {
        return new Label(stuffCatDef.LabelCap);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).label.CompareTo(GetValueCached(thing2).label);
    }
}
