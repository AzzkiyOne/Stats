using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_DirectHitChance : ColumnWorker_Num
{
    protected override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return 1f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius);
        }

        return 0f;
    }
}
