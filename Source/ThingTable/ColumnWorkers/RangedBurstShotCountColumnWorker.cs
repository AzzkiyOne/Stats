namespace Stats.ThingTable;

public sealed class RangedBurstShotCountColumnWorker : NumberColumnWorker<ThingAlike>
{
    public RangedBurstShotCountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    public static RangedBurstShotCountColumnWorker Make(ColumnDef columnDef) => new(columnDef);
    protected override decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return verb.burstShotCount;
        }

        return 0m;
    }
}
