using Verse;

namespace Stats.ThingTable;

public sealed class MeleeWeaponsTableWorker : TableWorker
{
    public MeleeWeaponsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsMeleeWeapon;
    }
}
