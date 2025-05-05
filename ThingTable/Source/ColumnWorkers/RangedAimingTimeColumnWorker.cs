using System;

namespace Stats.ThingTable;

public static class RangedAimingTimeColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue.Memoized(), " s");
    private static readonly Func<ThingAlike, decimal> GetValue = thing =>
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.warmupTime == null)
        {
            return 0m;
        }

        return verb.warmupTime.ToDecimal("F2");
    };
}
