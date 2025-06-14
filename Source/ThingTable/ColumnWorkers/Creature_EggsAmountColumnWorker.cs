using RimWorld;

namespace Stats.ThingTable;

public sealed class Creature_EggsAmountColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Creature_EggsAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps != null)
        {
            return eggLayerCompProps.eggCountRange.Average.ToDecimal(0);
        }

        return 0m;
    }
}
