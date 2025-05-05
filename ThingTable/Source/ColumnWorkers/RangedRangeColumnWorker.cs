using System;

namespace Stats.ThingTable;

public static class RangedRangeColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue.Memoized());
    private static readonly Func<ThingAlike, decimal> GetValue = thing =>
    {
        return thing.Def.Verbs.Primary()?.range.ToDecimal() ?? 0m;
    };
}
