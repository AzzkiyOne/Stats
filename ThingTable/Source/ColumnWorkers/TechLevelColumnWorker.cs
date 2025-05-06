using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Widgets;

namespace Stats.ThingTable;

public sealed class TechLevelColumnWorker : ColumnWorker<ThingAlike>
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
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var techLevels = tableRecords
            .Select(thing => thing.Def.techLevel)
            .Distinct()
            .OrderBy(techLevel => techLevel);

        return new OneToManyFilter<ThingAlike, TechLevel>(
            thing => thing.Def.techLevel,
            techLevels,
            techLevel => new Label(techLevel.ToString())
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return thing1.Def.techLevel.CompareTo(thing2.Def.techLevel);
    }
}
