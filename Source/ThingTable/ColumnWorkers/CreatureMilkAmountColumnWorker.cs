using RimWorld;

namespace Stats.ThingTable;

public sealed class CreatureMilkAmountColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureMilkAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            return milkableCompProps.milkAmount;
        }

        return 0m;
    }
}
