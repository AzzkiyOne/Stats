using System;

namespace Stats.ThingTable;

public static class RangedStoppingPowerColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue.Memoized());
    private static readonly Func<ThingAlike, decimal> GetValue = thing =>
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower.ToDecimal("F1") ?? 0m;
    };
}
