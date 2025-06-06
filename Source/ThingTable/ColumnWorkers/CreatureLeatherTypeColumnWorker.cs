using Verse;

namespace Stats.ThingTable;

public sealed class CreatureLeatherTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public CreatureLeatherTypeColumnWorker(ColumnDef columnDef) : base(columnDef, false)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.race?.leatherDef;
    }
}
