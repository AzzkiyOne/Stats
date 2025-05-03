using System;
using System.Linq;
using RimWorld;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public sealed class TechLevelColumnWorker : ColumnWorker
{
    private TechLevelColumnWorker() : base(TableColumnCellStyle.String)
    {
    }
    public static TechLevelColumnWorker Make(ColumnDef _) => new();
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        if (thing.Def.techLevel == TechLevel.Undefined)
        {
            return null;
        }

        return new Label(thing.Def.techLevel.ToString());
    }
    public override FilterWidget GetFilterWidget()
    {
        return new OneToManyFilter<TechLevel>(
            thing => thing.Def.techLevel,
            Enum.GetValues(typeof(TechLevel)).Cast<TechLevel>().OrderBy(techLevel => techLevel),
            techLevel => new Label(techLevel.ToString())
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return thing1.Def.techLevel.CompareTo(thing2.Def.techLevel);
    }
}
