using RimWorld;
using Verse;

namespace Stats;

public class TableWorker_Armor : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsWithinCategory(ThingCategoryDefOf.ApparelArmor);
    }
}
