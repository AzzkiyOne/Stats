namespace Stats.ThingTable;

public sealed class RangedRangeColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public RangedRangeColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return thing.Def.Verbs.Primary()?.range.ToString("F0") ?? "";
    }
}
