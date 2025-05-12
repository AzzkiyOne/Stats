using Verse;

namespace Stats.ThingTable;

public sealed class UtilityApparelTableWorker : TableWorker
{
    private readonly ThingCategoryDef UtilityCatDef =
        DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility");
    public UtilityApparelTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsApparel && thingDef.IsWithinCategory(UtilityCatDef);
    }
}
