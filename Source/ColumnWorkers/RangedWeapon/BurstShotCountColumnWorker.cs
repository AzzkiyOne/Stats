namespace Stats.ColumnWorkers.RangedWeapon;

public static class BurstShotCountColumnWorker
{
    public static NumberColumnWorker Make(ColumnDef _) => new(GetValue);
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return verb.burstShotCount;
        }

        return 0m;
    }
}
