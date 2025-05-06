using System;
using RimWorld;

namespace Stats.ThingTable;

public static class MaxChargesCountColumnWorker
{
    public static NumberColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue.Memoized());
    private static readonly Func<ThingAlike, decimal> GetValue = thing =>
    {
        var reloadableCompProperties = thing.Def.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties == null)
        {
            return 0m;
        }

        return reloadableCompProperties.maxCharges;
    };
}
