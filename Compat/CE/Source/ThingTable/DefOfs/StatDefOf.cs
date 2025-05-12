using RimWorld;

namespace Stats.Compat.CE.ThingTable;

[DefOf]
public static class StatDefOf
{
    public static StatDef Caliber;
#pragma warning disable CS8618
    static StatDefOf()
#pragma warning restore CS8618
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(StatDefOf));
    }
}
