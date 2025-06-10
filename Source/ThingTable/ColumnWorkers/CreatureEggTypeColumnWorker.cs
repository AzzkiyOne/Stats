using RimWorld;
using Verse;

namespace Stats.ThingTable;

public sealed class CreatureEggTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public CreatureEggTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps != null)
        {
            return eggLayerCompProps.eggUnfertilizedDef ?? eggLayerCompProps.eggFertilizedDef;
        }

        return null;
    }
}
