using Stats.ColumnWorkers;
using Verse;

namespace Stats.ThingTable;

public sealed class RangedDirectHitChanceColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public static RangedDirectHitChanceColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return (1f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius)).ToStringPercent();
        }

        return "";
    }
}
