namespace Stats.ThingTable;

public sealed class FuelConsumptionRateColumnWorker : NumberColumnWorker<ThingAlike>
{
    public FuelConsumptionRateColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var refuelableCompProps = thing.Def.GetRefuelableCompProperties();

        if (refuelableCompProps == null)
        {
            return 0m;
        }

        // TODO: Difficulty scaling.
        return refuelableCompProps.fuelConsumptionRate.ToDecimal(1);
    }
}
