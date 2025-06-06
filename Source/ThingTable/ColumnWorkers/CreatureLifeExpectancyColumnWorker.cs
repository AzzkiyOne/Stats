namespace Stats.ThingTable;

public sealed class CreatureLifeExpectancyColumnWorker : NumberColumnWorker<ThingAlike>
{
    public CreatureLifeExpectancyColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 y")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        return thing.Def.race?.lifeExpectancy.ToDecimal(0) ?? 0m;
    }
}
