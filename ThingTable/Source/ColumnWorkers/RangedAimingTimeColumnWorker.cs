using Stats.ColumnWorkers;
using Verse;

namespace Stats.ThingTable;

public sealed class RangedAimingTimeColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public static RangedAimingTimeColumnWorker Make(ColumnDef _) => new();
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
