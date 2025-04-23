using Stats.ColumnWorkers.Generic;

namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class BuildingDamageFactorImpassableColumnWorker : NumberColumnWorker<float>
{
    protected override float GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth is null or false)
        {
            return 0f;
        }

        return defaultProj.damageDef.buildingDamageFactorImpassable;
    }
}
