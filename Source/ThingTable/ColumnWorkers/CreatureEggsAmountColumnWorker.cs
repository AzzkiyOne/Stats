using RimWorld;

namespace Stats.ThingTable;

public sealed class CreatureEggsAmountColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureEggsAmountColumnWorker(ColumnDef columndef) : base(columndef)
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
