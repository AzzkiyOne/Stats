using RimWorld;
using Verse;

namespace Stats;

public static class TableFilters
{
    public static bool IsWeaponMelee(ThingDef thingDef)
    {
        return thingDef.IsMeleeWeapon;
    }
    public static bool IsWeaponRanged(ThingDef thingDef)
    {
        return thingDef.IsRangedWeapon;
    }
    public static bool IsApparelClothing(ThingDef thingDef)
    {
        return thingDef.IsApparel && !IsApparelArmor(thingDef) && !IsApparelUtility(thingDef);
    }
    public static bool IsApparelArmor(ThingDef thingDef)
    {
        return thingDef.IsWithinCategory(ThingCategoryDefOf.ApparelArmor);
    }
    public static bool IsApparelUtility(ThingDef thingDef)
    {
        return thingDef.IsApparel && thingDef.IsWithinCategory(DefDatabase<ThingCategoryDef>.GetNamed("ApparelUtility"));
    }
    public static bool IsAnimal(ThingDef thingDef)
    {
        return thingDef.race?.Animal == true && thingDef.IsCorpse == false;
    }
    public static bool IsStuff(ThingDef thingDef)
    {
        // This is to exclude chunks.
        return thingDef.stuffProps?.categories.Count > 0;
    }
    public static bool IsPlant(ThingDef thingDef)
    {
        return thingDef.IsPlant;
    }
    public static bool IsFood(ThingDef thingDef)
    {
        return thingDef.statBases?.Any(statBase =>
            statBase?.stat == StatDefOf.Nutrition
            && statBase.value > 0f
        ) ?? false;
    }
    public static bool IsDrug(ThingDef thingDef)
    {
        return thingDef.IsDrug;
    }
}
