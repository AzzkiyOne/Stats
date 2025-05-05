using CombatExtended;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.ThingTable.Defs;

namespace Stats.ThingTable.Compat.CE.ColumnWorkers;

public static class RangedReloadTimeColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue, " s");
    private static decimal GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statReq))
        {
            return CE_StatDefOf.ReloadTime.Worker.GetValue(statReq).ToDecimal("F2");
        }

        return 0m;
    }
}
