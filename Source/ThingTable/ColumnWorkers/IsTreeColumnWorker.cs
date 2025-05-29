namespace Stats.ThingTable;

public sealed class IsTreeColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public IsTreeColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.IsTree == true;
    }
}
