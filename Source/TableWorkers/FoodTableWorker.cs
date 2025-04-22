using RimWorld;
using Verse;

namespace Stats.TableWorkers;

public sealed class FoodTableWorker
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.statBases?.Any(statBase =>
            statBase?.stat == StatDefOf.Nutrition
            && statBase.value > 0f
        ) ?? false;
    }
}
