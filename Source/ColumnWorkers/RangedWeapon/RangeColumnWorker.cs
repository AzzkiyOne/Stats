namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class RangeColumnWorker : NumberColumnWorker
{
    public RangeColumnWorker() : base(thing => thing.Def.Verbs.Primary()?.range.ToDecimal() ?? 0m)
    {
    }
}
