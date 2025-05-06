using System;
using RimWorld;

namespace Stats.ThingTable;

public static class IsDestroyedWhenAllChargesSpentColumnWorker
{
    public static BooleanColumnWorker<ThingAlike> Make(ColumnDef _) => new(GetValue);
    private static readonly Func<ThingAlike, bool> GetValue = thing =>
    {
        var reloadableCompProperties = thing.Def.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties == null)
        {
            return false;
        }

        return reloadableCompProperties.destroyOnEmpty;
    };
}
