using Stats.ColumnWorkers.Generic;

namespace Stats.ColumnWorkers.RangedWeapon;

public class StoppingPowerColumnWorker
    : NumberColumnWorker<float>
{
    protected override float GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower ?? 0f;
    }
}
