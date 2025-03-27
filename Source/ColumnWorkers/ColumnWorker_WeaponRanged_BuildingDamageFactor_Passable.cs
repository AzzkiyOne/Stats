namespace Stats;

public class ColumnWorker_WeaponRanged_BuildingDamageFactor_Passable
    : ColumnWorker_Num
{
    public override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth is null or false) return 0f;

        return defaultProj.damageDef.buildingDamageFactorPassable;
    }
}
