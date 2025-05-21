using Verse;

namespace Stats.ThingTable;

public sealed class RangedImpassableBuildingDamageFactorColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public RangedImpassableBuildingDamageFactorColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
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
