using RimWorld;
using Verse;

namespace Stats;

public class TableWorker_Clothing : TableWorker
{
    private readonly ThingCategoryDef UtilityCatDef = DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility");
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsApparel
            && !thingDef.IsWithinCategory(ThingCategoryDefOf.ApparelArmor)
            && !thingDef.IsWithinCategory(UtilityCatDef);
    }
}
