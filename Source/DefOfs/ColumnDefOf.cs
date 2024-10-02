using RimWorld;

namespace Stats;

[DefOf]
public static class ColumnDefOf
{
    public static ColumnDef_Thing Id;
    static ColumnDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}
