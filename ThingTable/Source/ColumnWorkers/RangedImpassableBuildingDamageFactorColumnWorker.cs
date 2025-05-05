using System;

namespace Stats.ThingTable;

public static class RangedImpassableBuildingDamageFactorColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue.Memoized(), "%");
    private static readonly Func<ThingAlike, decimal> GetValue = thing =>
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return (defaultProj.damageDef.buildingDamageFactorImpassable * 100f).ToDecimal("F0");
        }

        return 0m;
    };
}
