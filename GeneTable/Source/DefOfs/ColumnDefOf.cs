using RimWorld;
using Stats.GeneTable.Defs;

namespace Stats.GeneTable.DefOfs;

[DefOf]
public static class ColumnDefOf
{
    public static ColumnDef Label_GeneTableColumn;
#pragma warning disable CS8618
    static ColumnDefOf()
#pragma warning restore CS8618
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}
