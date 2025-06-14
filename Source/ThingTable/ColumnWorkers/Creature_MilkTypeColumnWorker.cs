using RimWorld;
using Verse;

namespace Stats.ThingTable;

public sealed class Creature_MilkTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Creature_MilkTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.GetCompProperties<CompProperties_Milkable>()?.milkDef;
    }
}
