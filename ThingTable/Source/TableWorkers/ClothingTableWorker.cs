using RimWorld;
using Stats.ThingTable.Defs;
using Verse;

namespace Stats.ThingTable.TableWorkers;

public sealed class ClothingTableWorker : TableWorker
{
    private readonly ThingCategoryDef UtilityCatDef = DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility");
    public static ClothingTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsApparel
            && !thingDef.IsWithinCategory(ThingCategoryDefOf.ApparelArmor)
            && !thingDef.IsWithinCategory(UtilityCatDef);
    }
}
