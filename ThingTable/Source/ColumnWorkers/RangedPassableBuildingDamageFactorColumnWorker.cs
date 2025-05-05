using Stats.ColumnWorkers;
using Stats.ThingTable.Defs;

namespace Stats.ThingTable.ColumnWorkers;

public static class RangedPassableBuildingDamageFactorColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue, "%");
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return (defaultProj.damageDef.buildingDamageFactorPassable * 100f).ToDecimal("F0");
        }

        return 0m;
    }
}
