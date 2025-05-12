using Stats.ColumnWorkers;
using Verse;

namespace Stats.ThingTable;

public sealed class RangedAimingTimeColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public RangedAimingTimeColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.warmupTime > 0f)
        {
            return $"{verb.warmupTime:0.##} {"LetterSecond".Translate()}";
        }

        return "";
    }
}
