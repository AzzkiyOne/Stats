using RimWorld;
using Verse;

namespace Stats.ThingTable;

public sealed class CreatureMilkTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public CreatureMilkTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.GetCompProperties<CompProperties_Milkable>()?.milkDef;
    }
}
