namespace Stats;

public class ColumnWorker_WeaponRanged_AimingTime : ColumnWorker_Num
{
    protected override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.warmupTime > 0f)
        {
            return verb.warmupTime;
        }

        return 0f;
    }
}
