namespace Stats;

public class ColumnWorker_WeaponRanged_ArmorPenetration : ColumnWorker_Num
{
    public override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef is { harmsHealth: true, armorCategory: not null })
        {
            return defaultProj.GetArmorPenetration(null);
        }
        else if (defaultProj == null && verb?.beamDamageDef != null)
        {
            return verb.beamDamageDef.defaultArmorPenetration;
        }

        return 0f;
    }
}
