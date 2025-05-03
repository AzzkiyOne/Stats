namespace Stats.ColumnWorkers.RangedWeapon;

public static class RangeColumnWorker
{
    public static NumberColumnWorker Make(ColumnDef _) =>
        new(thing => thing.Def.Verbs.Primary()?.range.ToDecimal() ?? 0m);
}
