using Stats.ColumnWorkers.Generic;

namespace Stats.ColumnWorkers.RangedWeapon;

public class AimingTimeColumnWorker
    : NumberColumnWorker<float>
{
    protected override float GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.warmupTime == null)
        {
            return 0f;
        }

        return verb.warmupTime;
    }
}
