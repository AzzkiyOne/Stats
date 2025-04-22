using RimWorld;
using Verse;

namespace Stats.TableWorkers;

public sealed class ArmorTableWorker
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsWithinCategory(ThingCategoryDefOf.ApparelArmor);
    }
}
