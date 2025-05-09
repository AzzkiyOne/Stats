using Stats.ColumnWorkers;
using Verse;

namespace Stats.ThingTable;

public sealed class RangedArmorPenetrationColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public static RangedArmorPenetrationColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef is { harmsHealth: true, armorCategory: not null })
        {
            return defaultProj.GetArmorPenetration(null).ToStringPercent();
        }
        else if (defaultProj == null && verb?.beamDamageDef != null)
        {
            return verb.beamDamageDef.defaultArmorPenetration.ToStringPercent();
        }

        return "";
    }
}
