using System;
using Verse;

namespace Stats.ThingTable;

public static class RangedRPMColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue.Memoized(), " rpm");
    private static readonly Func<ThingAlike, decimal> GetValue = thing =>
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return (60f / verb.ticksBetweenBurstShots.TicksToSeconds()).ToDecimal();
        }

        return 0m;
    };
}
