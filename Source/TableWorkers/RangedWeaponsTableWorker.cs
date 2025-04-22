using Verse;

namespace Stats.TableWorkers;

public sealed class RangedWeaponsTableWorker
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsRangedWeapon;
    }
}
