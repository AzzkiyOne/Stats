using RimWorld;

namespace Stats.ThingTable.Compat.CE;

[DefOf]
public static class StatDefOf
{
    public static StatDef MeleeWeapon_AverageArmorPenetration;
    public static StatDef Caliber;
    public static StatDef MeleeDamage;
#pragma warning disable CS8618
    static StatDefOf()
#pragma warning restore CS8618
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(StatDefOf));
    }
}
