using Verse;

namespace Stats.ThingTable;

public sealed class Creature_MeatTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Creature_MeatTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.race?.meatDef;
    }
}
