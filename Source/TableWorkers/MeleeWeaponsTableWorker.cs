using Verse;

namespace Stats.TableWorkers;

public sealed class MeleeWeaponsTableWorker : TableWorker
{
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsMeleeWeapon;
    }
}
