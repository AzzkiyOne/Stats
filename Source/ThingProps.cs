using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats;

public static class ThingProps
{
    public static float WeaponRanged_Damage(ThingDef thingDef, ThingDef? stuffDef)
    {
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.GetDamageAmount(thingDef, stuffDef);
        }

        return 0f;
    }
    public static float WeaponRanged_StoppingPower(ThingDef thingDef, ThingDef? _)
    {
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower ?? 0f;
    }
    public static float WeaponRanged_ArmorPenetration(ThingDef thingDef, ThingDef? _)
    {
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef is { harmsHealth: true, armorCategory: not null })
        {
            return defaultProj.GetArmorPenetration(null);
        }
        else if (defaultProj == null && verb?.beamDamageDef != null)
        {
            return verb.beamDamageDef.defaultArmorPenetration;
        }

        return 0f;
    }
    public static float WeaponRanged_RPM(ThingDef thingDef, ThingDef? _)
    {
        var verb = thingDef.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return 60f / verb.ticksBetweenBurstShots.TicksToSeconds();
        }

        return 0f;
    }
    public static float WeaponRanged_BurstShotCount(ThingDef thingDef, ThingDef? _)
    {
        var verb = thingDef.Verbs.Primary();

        if (verb is { Ranged: true, showBurstShotStats: true, burstShotCount: > 1 })
        {
            return verb.burstShotCount;
        }

        return 0f;
    }
    public static float WeaponRanged_Range(ThingDef thingDef, ThingDef? _)
    {
        return thingDef.Verbs.FirstOrFallback(v => v?.isPrimary == true)?.range ?? 0f;
    }
    public static float WeaponRanged_AimingTime(ThingDef thingDef, ThingDef? _)
    {
        var verb = thingDef.Verbs.Primary();

        if (verb?.warmupTime > 0f)
        {
            return verb.warmupTime;
        }

        return 0f;
    }
    public static List<ThingAlike> CreatedAt(ThingDef thingDef, ThingDef? _)
    {
        var result = new HashSet<ThingAlike>();

        foreach (var recipe in DefDatabase<RecipeDef>.AllDefs)
        {
            if (recipe is { products.Count: 1, IsSurgery: false })
            {
                if (recipe.products.First().thingDef == thingDef)
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
    public static float WeaponRanged_BuildingDamageFactor_Passable(ThingDef thingDef, ThingDef? _)
    {
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.damageDef.buildingDamageFactorPassable;
        }

        return 0f;
    }
    public static float WeaponRanged_BuildingDamageFactor_Impassable(ThingDef thingDef, ThingDef? _)
    {
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.damageDef.buildingDamageFactorImpassable;
        }

        return 0f;
    }
    public static float WeaponRanged_DirectHitChance(ThingDef thingDef, ThingDef? _)
    {
        var verb = thingDef.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return 1f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius);
        }

        return 0f;
    }
    public static float WeaponRanged_MissRadius(ThingDef thingDef, ThingDef? _)
    {
        var verb = thingDef.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return verb.ForcedMissRadius;
        }

        return 0f;
    }
    public static bool IsWeapon(ThingDef thingDef, ThingDef? _)
    {
        return thingDef.IsWeapon;
    }
    public static bool IsMeleeWeapon(ThingDef thingDef, ThingDef? _)
    {
        return thingDef.IsMeleeWeapon;
    }
    public static bool IsRangedWeapon(ThingDef thingDef, ThingDef? _)
    {
        return thingDef.IsRangedWeapon;
    }
    public static bool IsApparel(ThingDef thingDef, ThingDef? _)
    {
        return thingDef.IsApparel;
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
    //public static bool? IsGun(ThingAlike thing)
    //{
    //    return !IsGrenade(thing);
    //}
    //public static bool? IsGrenade(ThingAlike thing)
    //{
    //    return thing.Def.IsWithinCategory(DefDatabase<ThingCategoryDef>.GetNamed("Grenades"));
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
