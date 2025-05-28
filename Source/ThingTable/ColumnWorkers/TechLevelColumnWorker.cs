using System.Collections.Generic;
using RimWorld;
using Stats.Widgets;

namespace Stats.ThingTable;

public sealed class TechLevelColumnWorker : ColumnWorker<ThingAlike>
{
    public TechLevelColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
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
        return Make.OTMFilter(thing => thing.Def.techLevel, tableRecords);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return thing1.Def.techLevel.CompareTo(thing2.Def.techLevel);
    }
}
