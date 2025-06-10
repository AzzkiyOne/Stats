using RimWorld;

namespace Stats.ThingTable;

public sealed class CreatureMilkingIntervalColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureMilkingIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            return milkableCompProps.milkIntervalDays;
        }

        return 0m;
    }
}
