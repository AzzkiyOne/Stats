using System;
using System.Linq;
using RimWorld;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public sealed class TechLevelColumnWorker : ColumnWorker<TechLevel>
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;

    protected override TechLevel GetValue(ThingAlike thing)
    {
        return thing.Def.techLevel;
    }
    protected override Widget GetTableCellContent(TechLevel techLevel, ThingAlike thing)
    {
        return TechLevelToWidget(techLevel);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new EnumerableFilter<TechLevel>(
            GetValueCached,
            Enum.GetValues(typeof(TechLevel)).Cast<TechLevel>().OrderBy(techLevel => techLevel),
            TechLevelToWidget
        );
    }
    private static Widget TechLevelToWidget(TechLevel techLevel)
    {
        return new Label(techLevel.ToString());
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).CompareTo(GetValueCached(thing2));
    }
}
