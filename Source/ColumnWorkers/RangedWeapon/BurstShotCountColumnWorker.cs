namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class BurstShotCountColumnWorker : NumberColumnWorker
{
    public BurstShotCountColumnWorker() : base(GetValue)
    {
    }
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
