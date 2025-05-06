using Verse;

namespace Stats.ThingTable;

public sealed class ApparelTableWorker : TableWorker
{
    private readonly ThingCategoryDef UtilityCatDef = DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility");
    public static ApparelTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsApparel && thingDef.IsWithinCategory(UtilityCatDef) == false;
    }
}
