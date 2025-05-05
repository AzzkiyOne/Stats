namespace Stats.ThingTable;

public static class RangedRangeColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) =>
        new(thing => thing.Def.Verbs.Primary()?.range.ToDecimal() ?? 0m);
}
