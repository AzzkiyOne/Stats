namespace Stats;

public class ColumnWorker_WeaponRanged_Damage
    : ColumnWorker_Num
{
    protected override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth is null or false)
        {
            return 0f;
        }

        return defaultProj.GetDamageAmount(thing.Def, thing.StuffDef);
    }
}
