namespace Stats.ThingTable;

public sealed class FuelCapacityColumnWorker : NumberColumnWorker<ThingAlike>
{
    public FuelCapacityColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var refuelableCompProps = thing.Def.GetRefuelableCompProperties();

        if (refuelableCompProps == null)
        {
            return 0m;
        }

        return refuelableCompProps.fuelCapacity.ToDecimal(0);
    }
}
