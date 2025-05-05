using RimWorld;
using Verse;

namespace Stats.ThingTable;

public sealed class FoodTableWorker : TableWorker
{
    public static FoodTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.statBases?.Any(statBase =>
            statBase?.stat == StatDefOf.Nutrition
            && statBase.value > 0f
        ) ?? false;
    }
}
