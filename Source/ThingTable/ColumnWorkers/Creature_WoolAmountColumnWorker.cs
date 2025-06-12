using RimWorld;

namespace Stats.ThingTable;

public sealed class Creature_WoolAmountColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Creature_WoolAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            return shearableCompProps.woolAmount;
        }

        return 0m;
    }
}
