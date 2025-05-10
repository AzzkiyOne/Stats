using RimWorld;

namespace Stats.ThingTable;

public sealed class IsDestroyedWhenAllChargesSpentColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public static IsDestroyedWhenAllChargesSpentColumnWorker Make(ColumnDef _) => new();
    protected override bool GetValue(ThingAlike thing)
    {
        var reloadableCompProperties = thing.Def.GetCompProperties<CompProperties_ApparelReloadable>();

        if (reloadableCompProperties == null)
        {
            return false;
        }

        return reloadableCompProperties.destroyOnEmpty;
    }
}
