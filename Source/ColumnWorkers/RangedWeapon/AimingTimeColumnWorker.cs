namespace Stats.ColumnWorkers.RangedWeapon;

public static class AimingTimeColumnWorker
{
    public static NumberColumnWorker Make(ColumnDef _) => new(GetValue, " s");
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
