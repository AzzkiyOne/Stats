namespace Stats.ThingTable;

public sealed class CanPlantBeGrownInHydroponicsColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public CanPlantBeGrownInHydroponicsColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.sowTags.Contains("Hydroponic") == true;
    }
}
