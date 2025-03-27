using Verse;

namespace Stats;

public class TableWorker_RangedWeapons
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsRangedWeapon;
    }
}
