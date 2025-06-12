namespace Stats.ThingTable;

public sealed class Creature_LifeExpectancyColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Creature_LifeExpectancyColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 y")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        return thing.Def.race?.lifeExpectancy.ToDecimal(0) ?? 0m;
    }
}
