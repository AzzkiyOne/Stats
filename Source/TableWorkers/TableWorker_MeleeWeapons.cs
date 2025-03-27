using Verse;

namespace Stats;

public class TableWorker_MeleeWeapons
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsMeleeWeapon;
    }
}
