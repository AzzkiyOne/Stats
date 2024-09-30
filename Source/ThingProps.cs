using RimWorld;
using Verse;

namespace Stats;

public static class ThingProps
{
    public static float? WeaponRange(ThingAlike thing)
    {
        return thing.Def.Verbs.FirstOrFallback(v => v?.isPrimary ?? false)?.range;
    }
    public static bool? IsMeal(ThingAlike thing)
    {
        return thing.Def.IsWithinCategory(DefDatabase<ThingCategoryDef>.GetNamed("FoodMeals"));
    }
    public static bool? IsProduce(ThingAlike thing)
    {
        return thing.Def.IsWithinCategory(ThingCategoryDefOf.PlantFoodRaw);
    }
    public static bool? IsMeat(ThingAlike thing)
    {
        return thing.Def.IsMeat;
    }
    public static ThingDef? Stuff(ThingAlike thing)
    {
        return thing.Stuff;
    }
}
