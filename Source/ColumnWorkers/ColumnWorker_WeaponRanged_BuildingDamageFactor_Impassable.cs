﻿namespace Stats;

public class ColumnWorker_WeaponRanged_BuildingDamageFactor_Impassable : ColumnWorker_Num
{
    protected override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.damageDef.buildingDamageFactorImpassable;
        }

        return 0f;
    }
}
