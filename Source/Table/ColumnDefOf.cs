using RimWorld;
using Stats.Table.Columns;

namespace Stats.Table;

[DefOf]
public static class ColumnDefOf
{
    public static Column_Label Label;
    static ColumnDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}
