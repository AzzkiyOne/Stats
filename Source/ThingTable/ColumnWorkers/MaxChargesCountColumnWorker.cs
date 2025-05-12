using RimWorld;

namespace Stats.ThingTable;

public sealed class MaxChargesCountColumnWorker : NumberColumnWorker<ThingAlike>
{
    public MaxChargesCountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var reloadableCompProperties = thing.Def.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties == null)
        {
            return 0m;
        }

        return reloadableCompProperties.maxCharges;
    }
}
