using Verse;

namespace Stats.ThingTable;

public sealed class Creature_LeatherTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Creature_LeatherTypeColumnWorker(ColumnDef columnDef) : base(columnDef, false)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.race?.leatherDef;
    }
}
