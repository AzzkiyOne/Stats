using Stats.ColumnWorkers.Generic;

namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class BurstShotCountColumnWorker
    : NumberColumnWorker<float>
{
    protected override float GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is not { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return 0f;
        }

        return verb.burstShotCount;
    }
}
