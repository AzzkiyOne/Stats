using System;
using Verse;

namespace Stats.ThingTable;

public static class RangedDirectHitChanceColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue.Memoized(), "%");
    private static readonly Func<ThingAlike, decimal> GetValue = thing =>
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius == null)
        {
            return 0m;
        }

        return (100f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius)).ToDecimal("F1");
    };
}
