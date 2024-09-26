using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats;

// Implement GetHashCode (and Equals) just in case?
public sealed class ThingAlike
{
    public ThingDef Def { get; }
    public ThingDef? Stuff { get; }
    public ThingAlike(ThingDef def, ThingDef? stuff = null)
    {
        Def = def;
        Stuff = stuff;
    }
    private static List<ThingAlike> _all;
    public static List<ThingAlike> All
    {
        get
        {
            if (_all != null)
            {
                return _all;
            }

            _all = [];

            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                if (
                    thingDef.IsBlueprint
                    || thingDef.IsFrame
                    || thingDef.isUnfinishedThing
                    || thingDef.IsCorpse
                    ||
                        thingDef.category != ThingCategory.Pawn
                        && thingDef.category != ThingCategory.Item
                        && thingDef.category != ThingCategory.Building
                        && thingDef.category != ThingCategory.Plant

                )
                {
                    continue;
                }

                if (thingDef.MadeFromStuff)
                {
                    var allowedStuffs = GenStuff.AllowedStuffsFor(thingDef);

                    foreach (var stuffDef in allowedStuffs)
                    {
                        _all.Add(new ThingAlike(thingDef, stuffDef));
                    }
                }
                else
                {
                    _all.Add(new ThingAlike(thingDef));
                }
            }

            return _all;
        }
    }
    //public static float? GetWeaponRange(ThingAlike thing)
    //{
    //    return thing.Def.Verbs.FirstOrFallback(v => v?.isPrimary ?? false)?.range;
    //}
    //public static bool? IsMeal(ThingAlike thing)
    //{
    //    return thing.Def.IsWithinCategory(DefDatabase<ThingCategoryDef>.GetNamed("FoodMeals"));
    //}
    //public static bool? IsProduce(ThingAlike thing)
    //{
    //    return thing.Def.IsWithinCategory(ThingCategoryDefOf.PlantFoodRaw);
    //}
    //public static bool? IsMeat(ThingAlike thing)
    //{
    //    return thing.Def.IsMeat;
    //}
}
