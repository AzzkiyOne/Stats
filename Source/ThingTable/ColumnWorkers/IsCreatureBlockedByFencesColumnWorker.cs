namespace Stats.ThingTable;

public sealed class IsCreatureBlockedByFencesColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public IsCreatureBlockedByFencesColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.race?.FenceBlocked == true;
    }
}
