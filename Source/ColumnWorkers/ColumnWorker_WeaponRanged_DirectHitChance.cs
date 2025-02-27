using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_DirectHitChance : ColumnWorker_Num
{
    public override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius == null) return 0f;

        return 1f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius);
    }
}
