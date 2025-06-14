using RimWorld;

namespace Stats.ThingTable;

public sealed class Creature_WoolShearingIntervalColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Creature_WoolShearingIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            return shearableCompProps.shearIntervalDays;
        }

        return 0m;
    }
}
