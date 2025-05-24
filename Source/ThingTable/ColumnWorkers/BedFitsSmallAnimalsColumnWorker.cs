namespace Stats.ThingTable;

public sealed class BedFitsSmallAnimalsColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public BedFitsSmallAnimalsColumnWorker(ColumnDef columndef) : base(columndef, false)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.building?.bed_humanlike == false;
    }
}
