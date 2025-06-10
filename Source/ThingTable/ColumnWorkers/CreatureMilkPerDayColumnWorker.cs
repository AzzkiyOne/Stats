using RimWorld;

namespace Stats.ThingTable;

public sealed class CreatureMilkPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureMilkPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            return ((float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays).ToDecimal(1);
        }

        return 0m;
    }
}
