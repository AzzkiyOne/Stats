using Verse;

namespace Stats.ColumnWorkers.RangedWeapon;

public static class DirectHitChanceColumnWorker
{
    public static NumberColumnWorker Make(ColumnDef _) => new(GetValue, "%");
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
