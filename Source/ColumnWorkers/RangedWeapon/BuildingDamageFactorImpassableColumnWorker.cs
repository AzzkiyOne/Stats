namespace Stats.ColumnWorkers.RangedWeapon;

public static class BuildingDamageFactorImpassableColumnWorker
{
    public static NumberColumnWorker Make(ColumnDef _) => new(GetValue, "%");
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return (defaultProj.damageDef.buildingDamageFactorImpassable * 100f).ToDecimal("F0");
        }

        return 0m;
    }
}
