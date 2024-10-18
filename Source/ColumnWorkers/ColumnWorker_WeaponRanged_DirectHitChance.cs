using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_DirectHitChance : ColumnWorker_Num
{
    protected override float GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var verb = thingDef.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return 1f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius);
        }

        return 0f;
    }
}
