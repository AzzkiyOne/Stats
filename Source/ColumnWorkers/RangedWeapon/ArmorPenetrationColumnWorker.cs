namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class ArmorPenetrationColumnWorker : NumberColumnWorker
{
    public ArmorPenetrationColumnWorker() : base(GetValue, "%")
    {
    }
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef is { harmsHealth: true, armorCategory: not null })
        {
            return (defaultProj.GetArmorPenetration(null) * 100f).ToDecimal("F0");
        }
        else if (defaultProj == null && verb?.beamDamageDef != null)
        {
            return (verb.beamDamageDef.defaultArmorPenetration * 100f).ToDecimal("F0");
        }

        return 0m;
    }
}
