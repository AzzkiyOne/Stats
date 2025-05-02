namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class DamageColumnWorker : NumberColumnWorker
{
    public DamageColumnWorker() : base(GetValue)
    {
    }
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.GetDamageAmount(thing.Def, thing.StuffDef);
        }

        return 0m;
    }
}
