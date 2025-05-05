using Stats.ColumnWorkers;
using Stats.ThingTable.Defs;
using Verse;

namespace Stats.ThingTable.ColumnWorkers.RangedWeapon;

public static class DirectHitChanceColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue, "%");
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius == null)
        {
            return 0m;
        }

        return (100f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius)).ToDecimal("F1");
    }
}
