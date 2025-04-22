using Stats.ColumnWorkers.Generic;

namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class ArmorPenetrationColumnWorker
    : NumberColumnWorker<float>
{
    protected override float GetValue(ThingAlike thing)
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
