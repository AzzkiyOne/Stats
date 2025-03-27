namespace Stats;

public class ColumnWorker_WeaponRanged_BurstShotCount
    : ColumnWorker_Num
{
    public override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is not { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 }) return 0f;

        return verb.burstShotCount;
    }
}
