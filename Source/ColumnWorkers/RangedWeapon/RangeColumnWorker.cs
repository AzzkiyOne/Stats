using Stats.ColumnWorkers.Generic;

namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class RangeColumnWorker : NumberColumnWorker<float>
{
    protected override float GetValue(ThingAlike thing)
    {
        return thing.Def.Verbs.Primary()?.range ?? 0f;
    }
}
