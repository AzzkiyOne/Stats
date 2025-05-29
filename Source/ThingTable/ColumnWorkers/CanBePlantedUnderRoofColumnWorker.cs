namespace Stats.ThingTable;

public sealed class CanBePlantedUnderRoofColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public CanBePlantedUnderRoofColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.interferesWithRoof == false;
    }
}
