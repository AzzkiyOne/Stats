using Stats.ColumnWorkers;

namespace Stats.ThingTable;

public sealed class RangedStoppingPowerColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public static RangedStoppingPowerColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower.ToString("F1") ?? "";
    }
}
