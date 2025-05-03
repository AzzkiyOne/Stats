using RimWorld;
using Verse;

namespace Stats.TableWorkers;

public sealed class ArmorTableWorker : TableWorker
{
    public static ArmorTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsWithinCategory(ThingCategoryDefOf.ApparelArmor);
    }
}
