using RimWorld;

namespace Stats;

[DefOf]
public static class TableDefOf
{
    public static TableDef All;
    static TableDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(TableDefOf));
    }
}
