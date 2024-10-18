using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_MissRadius : ColumnWorker_Num
{
    protected override float GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var verb = thingDef.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return verb.ForcedMissRadius;
        }

        return 0f;
    }
}
