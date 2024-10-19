namespace Stats;

public class ColumnWorker_WeaponRanged_BurstShotCount : ColumnWorker_Num
{
    protected override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return verb.burstShotCount;
        }

        return 0f;
    }
}
