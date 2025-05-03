using Verse;

namespace Stats.TableWorkers;

public sealed class MeleeWeaponsTableWorker : TableWorker
{
    public static MeleeWeaponsTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsMeleeWeapon;
    }
}
