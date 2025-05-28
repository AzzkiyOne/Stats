using Verse;

namespace Stats.ThingTable;

public sealed class FuelTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public FuelTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.GetRefuelableCompProperties()?.fuelFilter?.AnyAllowedDef;
    }
}
