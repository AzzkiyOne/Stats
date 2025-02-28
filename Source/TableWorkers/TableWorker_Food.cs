using RimWorld;
using Verse;

namespace Stats;

public class TableWorker_Food : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.statBases?.Any(statBase =>
            statBase?.stat == StatDefOf.Nutrition
            && statBase.value > 0f
        ) ?? false;
    }
}
