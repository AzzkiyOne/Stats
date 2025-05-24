namespace Stats.ThingTable;

public sealed class PowerPerFuelColumnWorker : NumberColumnWorker<ThingAlike>
{
    public PowerPerFuelColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 W/u")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var powerCompProps = thing.Def.GetPowerCompProperties();
        var refuelableCompProps = thing.Def.GetRefuelableCompProperties();

        if
        (
            powerCompProps == null
            || refuelableCompProps == null
            || refuelableCompProps.fuelConsumptionRate == 0f
        )
        {
            return 0m;
        }

        var powerOutput = powerCompProps.PowerConsumption * -1f;
        var fuelConsumptionRate = refuelableCompProps.fuelConsumptionRate;

        return (powerOutput / fuelConsumptionRate).ToDecimal(0);
    }
}
