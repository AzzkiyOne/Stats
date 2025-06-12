using RimWorld;

namespace Stats.ThingTable;

public sealed class Creature_EggsPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Creature_EggsPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps != null)
        {
            return (eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays).ToDecimal(1);
        }

        return 0m;
    }
}
