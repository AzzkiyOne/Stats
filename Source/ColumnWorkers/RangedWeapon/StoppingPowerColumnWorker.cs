namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class StoppingPowerColumnWorker : NumberColumnWorker
{
    public StoppingPowerColumnWorker() : base(GetValue)
    {
    }
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower.ToDecimal("F1") ?? 0m;
    }
}
