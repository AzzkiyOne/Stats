using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_AimingTime : ColumnWorker_Num
{
    protected override float GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var verb = thingDef.Verbs.Primary();

        if (verb?.warmupTime > 0f)
        {
            return verb.warmupTime;
        }

        return 0f;
    }
}
