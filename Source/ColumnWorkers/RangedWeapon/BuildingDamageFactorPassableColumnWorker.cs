namespace Stats.ColumnWorkers.RangedWeapon;

public static class BuildingDamageFactorPassableColumnWorker
{
    public static NumberColumnWorker Make(ColumnDef _) => new(GetValue, "%");
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return (defaultProj.damageDef.buildingDamageFactorPassable * 100f).ToDecimal("F0");
        }

        return 0m;
    }
}
