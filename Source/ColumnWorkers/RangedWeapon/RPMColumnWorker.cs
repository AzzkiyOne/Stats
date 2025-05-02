using Verse;

namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class RPMColumnWorker : NumberColumnWorker
{
    public RPMColumnWorker() : base(GetValue, " rpm")
    {
    }
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return (60f / verb.ticksBetweenBurstShots.TicksToSeconds()).ToDecimal();
        }

        return 0m;
    }
}
