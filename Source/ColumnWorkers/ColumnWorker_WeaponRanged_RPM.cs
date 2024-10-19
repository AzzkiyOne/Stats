using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_RPM : ColumnWorker_Num
{
    protected override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return 60f / verb.ticksBetweenBurstShots.TicksToSeconds();
        }

        return 0f;
    }
}
