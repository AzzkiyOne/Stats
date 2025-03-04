using RimWorld;

namespace Stats;

[DefOf]
public static class ColumnDefOf
{
    public static ColumnDef Name;
#pragma warning disable CS8618
    static ColumnDefOf()
#pragma warning restore CS8618
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}
