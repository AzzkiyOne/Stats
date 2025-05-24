using RimWorld;
using Verse;

namespace Stats.ThingTable;

public sealed class RecreationalBuildingsTableWorker : TableWorker
{
    public RecreationalBuildingsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.statBases?.GetStatValueFromList(StatDefOf.JoyGainFactor, 0f) > 0f;
    }
}
