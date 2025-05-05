using Stats.ColumnWorkers;
using Stats.ThingTable.Defs;

namespace Stats.ThingTable.ColumnWorkers;

public static class RangedStoppingPowerColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue);
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower.ToDecimal("F1") ?? 0m;
    }
}
