using Verse;

namespace Stats.ThingTable;

public sealed class PlantProductTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public PlantProductTypeColumnWorker(ColumnDef columnDef) : base(columnDef, false)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.harvestedThingDef;
    }
}
