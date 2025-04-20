using Stats.ColumnWorkers.Generic;
using Verse;

namespace Stats.ColumnWorkers.RangedWeapon;

public class RPMColumnWorker
    : NumberColumnWorker<float>
{
    protected override float GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is not { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return 0f;
        }

        return 60f / verb.ticksBetweenBurstShots.TicksToSeconds();
    }
}
