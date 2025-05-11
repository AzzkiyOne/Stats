using Stats.ColumnWorkers;
using Verse;

namespace Stats.ThingTable;

public sealed class RangedImpassableBuildingDamageFactorColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public static RangedImpassableBuildingDamageFactorColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var damageDef = verb?.defaultProjectile?.projectile?.damageDef;

        if (damageDef != null && damageDef.buildingDamageFactorImpassable != 1f)
        {
            return damageDef.buildingDamageFactorImpassable.ToStringPercent();
        }

        return "";
    }
}
