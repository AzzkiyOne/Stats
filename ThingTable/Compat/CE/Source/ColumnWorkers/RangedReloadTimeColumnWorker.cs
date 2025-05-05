using CombatExtended;
using RimWorld;

namespace Stats.ThingTable.Compat.CE;

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
