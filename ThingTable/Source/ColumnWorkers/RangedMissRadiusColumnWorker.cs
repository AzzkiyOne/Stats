using System;

namespace Stats.ThingTable;

public static class RangedMissRadiusColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue.Memoized());
    private static readonly Func<ThingAlike, decimal> GetValue = thing =>
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius == null)
        {
            return 0m;
        }

        return verb.ForcedMissRadius.ToDecimal("F1");
    };
}
