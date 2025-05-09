using Stats.ColumnWorkers;

namespace Stats.ThingTable;

public sealed class RangedRangeColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public static RangedRangeColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return thing.Def.Verbs.Primary()?.range.ToString("F0") ?? "";
    }
}
