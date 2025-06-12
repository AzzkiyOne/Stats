using Verse;

namespace Stats.ThingTable;

public sealed class Creature_TrainabilityColumnWorker : DefColumnWorker<ThingAlike, TrainabilityDef?>
{
    public Creature_TrainabilityColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override TrainabilityDef? GetValue(ThingAlike thing)
    {
        return thing.Def.race?.trainability;
    }
}
