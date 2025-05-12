using Verse;

namespace Stats.ThingTable;

public sealed class RangedWeaponsTableWorker : TableWorker
{
    public RangedWeaponsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsRangedWeapon;
    }
}
