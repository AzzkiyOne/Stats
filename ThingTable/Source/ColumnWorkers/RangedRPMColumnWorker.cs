using Stats.ColumnWorkers;
using Stats.ThingTable.Defs;
using Verse;

namespace Stats.ThingTable.ColumnWorkers;

public static class RangedRPMColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue, " rpm");
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return (60f / verb.ticksBetweenBurstShots.TicksToSeconds()).ToDecimal();
        }

        return 0m;
    }
}
