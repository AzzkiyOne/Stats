using Stats.ColumnWorkers;
using Verse;

namespace Stats.ThingTable;

public sealed class RangedRPMColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public static RangedRPMColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return (60f / verb.ticksBetweenBurstShots.TicksToSeconds()).ToString("0.## rpm");
        }

        return "";
    }
}
