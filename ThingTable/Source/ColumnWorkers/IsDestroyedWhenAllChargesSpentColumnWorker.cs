using RimWorld;

namespace Stats.ThingTable;

public sealed class IsDestroyedWhenAllChargesSpentColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public IsDestroyedWhenAllChargesSpentColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        var reloadableCompProperties = thing.Def.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties == null)
        {
            return false;
        }

        return reloadableCompProperties.destroyOnEmpty;
    }
}
