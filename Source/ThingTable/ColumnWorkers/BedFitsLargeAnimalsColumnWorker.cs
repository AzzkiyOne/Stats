namespace Stats.ThingTable;

public sealed class BedFitsLargeAnimalsColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public BedFitsLargeAnimalsColumnWorker(ColumnDef columndef) : base(columndef, false)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        var buildingProps = thing.Def.building;

        if (buildingProps == null)
        {
            return false;
        }

        return buildingProps.bed_humanlike == false && buildingProps.bed_maxBodySize > 0.55f;
    }
}
