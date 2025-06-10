using RimWorld;

namespace Stats.ThingTable;

public sealed class CreatureWoolShearingIntervalColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureWoolShearingIntervalColumnWorker(ColumnDef columndef) : base(columndef)
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
