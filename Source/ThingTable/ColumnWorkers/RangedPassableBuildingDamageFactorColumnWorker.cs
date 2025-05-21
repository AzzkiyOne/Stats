using Verse;

namespace Stats.ThingTable;

public sealed class RangedPassableBuildingDamageFactorColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public RangedPassableBuildingDamageFactorColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var damageDef = verb?.defaultProjectile?.projectile?.damageDef;

        if (damageDef != null && damageDef.buildingDamageFactorPassable != 1f)
        {
            return damageDef.buildingDamageFactorPassable.ToStringPercent();
        }

        return "";
    }
}
