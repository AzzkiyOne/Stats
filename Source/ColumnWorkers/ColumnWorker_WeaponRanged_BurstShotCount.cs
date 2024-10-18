using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_BurstShotCount : ColumnWorker_Num
{
    protected override float GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var verb = thingDef.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return verb.burstShotCount;
        }

        return 0f;
    }
}
