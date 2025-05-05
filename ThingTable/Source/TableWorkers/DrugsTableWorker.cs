using Stats.ThingTable.Defs;
using Verse;

namespace Stats.ThingTable.TableWorkers;

public sealed class DrugsTableWorker : TableWorker
{
    public static DrugsTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsDrug;
    }
}
