using RimWorld;
using Verse;

namespace Stats.ThingTable;

public sealed class CreatureWoolTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public CreatureWoolTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.GetCompProperties<CompProperties_Shearable>()?.woolDef;
    }
}
