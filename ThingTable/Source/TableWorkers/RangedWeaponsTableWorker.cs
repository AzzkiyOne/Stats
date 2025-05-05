using Verse;

namespace Stats.ThingTable;

public sealed class RangedWeaponsTableWorker : TableWorker
{
    public static RangedWeaponsTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsRangedWeapon;
    }
}
