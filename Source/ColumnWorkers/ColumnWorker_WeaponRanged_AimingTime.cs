namespace Stats;

public class ColumnWorker_WeaponRanged_AimingTime
    : ColumnWorker_Num
{
    public override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.warmupTime == null) return 0f;

        return verb.warmupTime;
    }
}
