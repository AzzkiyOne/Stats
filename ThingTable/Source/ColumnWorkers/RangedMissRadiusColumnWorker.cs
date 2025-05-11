using Stats.ColumnWorkers;

namespace Stats.ThingTable;

public sealed class RangedMissRadiusColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public static RangedMissRadiusColumnWorker Make(ColumnDef _) => new();
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return verb.ForcedMissRadius.ToString("0.#");
        }

        return "";
    }
}
