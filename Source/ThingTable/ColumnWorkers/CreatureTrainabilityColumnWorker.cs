using Verse;

namespace Stats.ThingTable;

public sealed class CreatureTrainabilityColumnWorker : DefColumnWorker<ThingAlike, TrainabilityDef?>
{
    public CreatureTrainabilityColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override TrainabilityDef? GetValue(ThingAlike thing)
    {
        return thing.Def.race?.trainability;
    }
}
