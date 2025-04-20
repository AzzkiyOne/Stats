using RimWorld;
using Verse;

namespace Stats.TableWorkers;

public class ClothingTableWorker
    : TableWorker
{
    private readonly ThingCategoryDef UtilityCatDef = DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility");
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsApparel
            && !thingDef.IsWithinCategory(ThingCategoryDefOf.ApparelArmor)
            && !thingDef.IsWithinCategory(UtilityCatDef);
    }
}
