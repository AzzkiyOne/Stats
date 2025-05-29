namespace Stats.ThingTable;

public sealed class IsPlantBlightableColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public IsPlantBlightableColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.Blightable == true;
    }
}
