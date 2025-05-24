namespace Stats.ThingTable;

public sealed class PowerPerCellColumnWorker : NumberColumnWorker<ThingAlike>
{
    public PowerPerCellColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 W/c")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var powerCompProps = thing.Def.GetPowerCompProperties();

        if (powerCompProps == null)
        {
            return 0m;
        }

        return powerCompProps.PowerConsumption.ToDecimal(0) * -1m / thing.Def.size.Area;
    }
}
