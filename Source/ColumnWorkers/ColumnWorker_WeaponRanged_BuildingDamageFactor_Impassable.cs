using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_BuildingDamageFactor_Impassable : ColumnWorker_Num
{
    protected override float GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.damageDef.buildingDamageFactorImpassable;
        }

        return 0f;
    }
}
