using Verse;

namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class DirectHitChanceColumnWorker : NumberColumnWorker
{
    public DirectHitChanceColumnWorker() : base(GetValue, "%")
    {
    }
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
