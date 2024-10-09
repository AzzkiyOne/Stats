using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Stats;

public static class ThingProps
{
    public static float? WeaponRanged_Damage(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.GetDamageAmount(thing.Def, thing.Stuff);
        }

        return null;
    }
    public static float? WeaponRanged_StoppingPower(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.stoppingPower != 0f)
        {
            return defaultProj?.stoppingPower;
        }

        return null;
    }
    public static float? WeaponRanged_ArmorPenetration(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef is { harmsHealth: true, armorCategory: not null })
        {
            return defaultProj.GetArmorPenetration(null);
        }
        else if (defaultProj == null && verb?.beamDamageDef != null)
        {
            return verb.beamDamageDef.defaultArmorPenetration;
        }

        return null;
    }
    public static float? WeaponRanged_RPM(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return 60f / verb.ticksBetweenBurstShots.TicksToSeconds();
        }

        return null;
    }
    public static float? WeaponRanged_BurstShotCount(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return verb.burstShotCount;
        }

        return null;
    }
    public static float? WeaponRanged_Range(ThingAlike thing)
    {
        return thing.Def.Verbs.FirstOrFallback(v => v?.isPrimary == true)?.range;
    }
    public static float? WeaponRanged_AimingTime(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.warmupTime > 0f)
        {
            return verb.warmupTime;
        }

        return null;
    }
    public static List<ThingAlike> CreatedAt(ThingAlike thing)
    {
        var result = new HashSet<ThingAlike>();

        foreach (var recipe in DefDatabase<RecipeDef>.AllDefs)
        {
            if (recipe is { products.Count: 1, IsSurgery: false })
            {
                if (recipe.products.First().thingDef == thing.Def)
                {
                    foreach (var recipeUser in recipe.AllRecipeUsers)
                    {
                        result.Add(recipeUser);
                    }
                }
            }
        }

        return result.ToList();
    }
    public static float? WeaponRanged_BuildingDamageFactor_Passable(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.damageDef.buildingDamageFactorPassable;
        }

        return null;
    }
    public static float? WeaponRanged_BuildingDamageFactor_Impassable(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.damageDef.buildingDamageFactorImpassable;
        }

        return null;
    }
    public static float? WeaponRanged_DirectHitChance(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return 1f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius);
        }

        return null;
    }
    public static float? WeaponRanged_MissRadius(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return verb.ForcedMissRadius;
        }

        return null;
    }
    //public static bool? IsFood(ThingDef def)
    //{
    //    return def.statBases?.Any(statBase =>
    //        statBase?.stat == StatDefOf.Nutrition
    //        && statBase.value > 0
    //    );
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
    //public static bool? IsAnimalProduct(ThingAlike thing)
    //{
    //    return thing.Def.IsAnimalProduct;
    //}
    public static bool? IsWeapon(ThingAlike thing)
    {
        return thing.Def.IsWeapon;
    }
    public static bool? IsMeleeWeapon(ThingAlike thing)
    {
        return thing.Def.IsMeleeWeapon;
    }
    public static bool? IsRangedWeapon(ThingAlike thing)
    {
        return thing.Def.IsRangedWeapon;
    }
    //public static bool? IsGun(ThingAlike thing)
    //{
    //    return !IsGrenade(thing);
    //}
    //public static bool? IsGrenade(ThingAlike thing)
    //{
    //    return thing.Def.IsWithinCategory(DefDatabase<ThingCategoryDef>.GetNamed("Grenades"));
    //}
    //public static bool? IsApparel(ThingAlike thing)
    //{
    //    return thing.Def.IsApparel;
    //}
    //public static bool? IsBuilding(ThingAlike thing)
    //{
    //    return thing.Def.category == ThingCategory.Building;
    //}
    //public static bool? IsCreature(ThingAlike thing)
    //{
    //    return thing.Def.race is not null;
    //}
    //public static bool? IsCreatureAnimal(ThingAlike thing)
    //{
    //    return thing.Def.race?.Animal == true;
    //}
    //public static bool? IsCreatureDomestic(ThingAlike thing)
    //{
    //    return thing.Def.race?.wildness == 0;
    //}
    //public static bool? IsCreatureWild(ThingAlike thing)
    //{
    //    return thing.Def.race?.wildness > 0;
    //}
    //public static bool? IsCreatureMech(ThingAlike thing)
    //{
    //    return thing.Def.race?.IsMechanoid == true;
    //}
    //public static bool? IsPlant(ThingAlike thing)
    //{
    //    return thing.Def.IsPlant;
    //}
    //public static bool? IsPlantSowable(ThingAlike thing)
    //{
    //    return thing.Def.plant?.Sowable;
    //}
    //public static bool? IsPlantWild(ThingAlike thing)
    //{
    //    return !IsPlantSowable(thing);
    //}
    //public static bool? IsPlantProduceYeilding(ThingAlike thing)
    //{
    //    //return def.plant?.humanFoodPlant == true;
    //    return thing.Def.plant?.harvestedThingDef?.statBases?.Any(statBase =>
    //        statBase?.stat == StatDefOf.Nutrition && statBase.value > 0
    //    );
    //}
    //public static bool? IsPlantTree(ThingAlike thing)
    //{
    //    return thing.Def.plant?.IsTree;
    //}
    //public static ThingDef? Stuff(ThingAlike thing)
    //{
    //    return thing.Stuff;
    //}
}
