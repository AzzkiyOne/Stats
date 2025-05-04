using RimWorld;
using Stats.ThingTable.Defs;

namespace Stats.ThingTable.DefOfs;

[DefOf]
public static class TableDefOf
{
    public static TableDef RangedWeapons;
#pragma warning disable CS8618
    static TableDefOf()
#pragma warning restore CS8618
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(TableDefOf));
    }
}
