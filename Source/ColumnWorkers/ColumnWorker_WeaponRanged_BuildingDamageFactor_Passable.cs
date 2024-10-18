using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_BuildingDamageFactor_Passable : ColumnWorker_Num
{
    protected override float GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.damageDef.buildingDamageFactorPassable;
        }

        return 0f;
    }
}
