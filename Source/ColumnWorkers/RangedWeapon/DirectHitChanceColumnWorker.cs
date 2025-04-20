using Stats.ColumnWorkers.Generic;
using Verse;

namespace Stats.ColumnWorkers.RangedWeapon;

public class DirectHitChanceColumnWorker
    : NumberColumnWorker<float>
{
    protected override float GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius == null)
        {
            return 0f;
        }

        return 1f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius);
    }
}
