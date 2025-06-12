using RimWorld;
using Verse;

namespace Stats.ThingTable;

public sealed class Creature_WoolTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Creature_WoolTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.GetCompProperties<CompProperties_Shearable>()?.woolDef;
    }
}
