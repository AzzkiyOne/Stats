using Stats.ColumnWorkers;
using Verse;

namespace Stats.ThingTable;

public sealed class RangedDirectHitChanceColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public RangedDirectHitChanceColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
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
