namespace Stats.ThingTable;

public sealed class IsPlantSowableColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public IsPlantSowableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.Sowable == true;
    }
}
