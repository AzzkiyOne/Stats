using RimWorld;
using Stats.Table.Columns;

namespace Stats.Table;

[DefOf]
public static class ColumnDefOf
{
    public static Column_Thing Id;
    static ColumnDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}
