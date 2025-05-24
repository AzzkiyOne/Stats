namespace Stats.ThingTable;

public sealed class PowerConsumptionColumnWorker : NumberColumnWorker<ThingAlike>
{
    public PowerConsumptionColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 W")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var powerCompProps = thing.Def.GetPowerCompProperties();

        if (powerCompProps == null || powerCompProps.PowerConsumption < 0f)
        {
            return 0m;
        }

        return powerCompProps.PowerConsumption.ToDecimal(0);
    }
}
