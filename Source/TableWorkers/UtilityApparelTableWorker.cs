using Verse;

namespace Stats.TableWorkers;

public sealed class UtilityApparelTableWorker : TableWorker
{
    private readonly ThingCategoryDef UtilityCatDef =
        DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility");
    public static UtilityApparelTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsApparel && thingDef.IsWithinCategory(UtilityCatDef);
    }
}
