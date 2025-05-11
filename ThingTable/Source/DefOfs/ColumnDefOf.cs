using RimWorld;

namespace Stats.ThingTable;

[DefOf]
public static class ColumnDefOf
{
    public static ColumnDef Label_ThingTableColumn;
#pragma warning disable CS8618
    static ColumnDefOf()
#pragma warning restore CS8618
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ColumnDefOf));
    }
}
