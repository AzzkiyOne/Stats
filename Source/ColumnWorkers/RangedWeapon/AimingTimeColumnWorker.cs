namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class AimingTimeColumnWorker : NumberColumnWorker
{
    public AimingTimeColumnWorker() : base(GetValue, " s")
    {
    }
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.warmupTime == null)
        {
            return 0m;
        }

        return verb.warmupTime.ToDecimal("F2");
    }
}
