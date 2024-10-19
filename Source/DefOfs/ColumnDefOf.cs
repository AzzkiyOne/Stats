using RimWorld;

namespace Stats;

[DefOf]
public static class ColumnDefOf
{
    public static ColumnDef Name;
    static ColumnDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}
