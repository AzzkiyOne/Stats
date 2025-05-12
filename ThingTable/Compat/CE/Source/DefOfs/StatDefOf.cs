using RimWorld;

namespace Stats.ThingTable.Compat.CE;

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
