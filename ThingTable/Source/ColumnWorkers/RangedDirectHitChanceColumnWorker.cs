using Verse;

namespace Stats.ThingTable;

public static class RangedDirectHitChanceColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue, "%");
    private static decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius == null)
        {
            return 0m;
        }

        return (100f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius)).ToDecimal("F1");
    }
}
