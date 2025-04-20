using Verse;

namespace Stats.TableWorkers;

public class RangedWeaponsTableWorker
    : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsRangedWeapon;
    }
}
