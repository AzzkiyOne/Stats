using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

// Why custom category system? Why not just make a wrapper around ThingCategoryDef then patch
// all of the things that do not have a category by adding a DefModExtension in which a
// category will be defined and add new categories with a custom class (like with columns)?
//
// - Imagine a mod adds a set of non-minifiable buildings. They will all need to be patched.
// - By adding categories through wrappers we are changing the structure anyway. So it will be
// no less confusing than a completely separate structure.
// - It will be hard to tell "Where does this category comes from?".
// - It will be dirty.

[DefOf]
public static class DynamicThingCategoryDefOf
{
    public static DynamicThingCategoryDef Root;

    static DynamicThingCategoryDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(DynamicThingCategoryDefOf));
    }
}

// Basically a tree of cascading filters.
public class DynamicThingCategoryDef : Def
{
    public DynamicThingCategoryDef? parent;
    public Texture2D Icon { get; } = BaseContent.BadTex;
    private List<DynamicThingCategoryDef> _children;
    public List<DynamicThingCategoryDef> Children
    {
        get
        {
            if (_children is null)
            {
                var childCategoryDefs = DefDatabase<DynamicThingCategoryDef>
                    .AllDefs
                    .Where(c => c.parent == this);

                _children = new(childCategoryDefs);

                if (_children.Count > 0)
                {
                    // This is a (temporary) crutch.
                    _children.Add(new DynamicThingCategoryDef()
                    {
                        parent = this,
                        label = "misc",
                        // I wonder if it'll be faster to check if a ThingDef is contained
                        // in any of the children items.
                        filter = (thingDef) => childCategoryDefs.All(
                            child => child.filter(thingDef) == false
                        )
                    });
                }

                _children.SortBy(child => child.label);
            }

            return _children;
        }
    }
    private List<ThingAlike>? _items;
    public List<ThingAlike> Items
    {
        get
        {
            if (_items is null)
            {
                if (parent is null)
                {
                    return ThingAlikes.list.Where((thing) => filter(thing.def)).ToList();
                }
                else
                {
                    _items = parent.Items.Where((thing) => filter(thing.def)).ToList();

                    return _items;
                }
            }

            return _items;
        }
    }
    public Func<ThingDef, bool> filter = (_) => true;
}

public static class DynamicThingCategoryFilters
{
    public static bool IsFood(ThingDef def)
    {
        return def.statBases?.Any(statBase =>
            statBase?.stat == StatDefOf.Nutrition
            && statBase.value > 0
        ) ?? false;
    }
    public static bool IsMeal(ThingDef def)
    {
        return def.IsWithinCategory(DefDatabase<ThingCategoryDef>.GetNamed("FoodMeals"));
    }
    public static bool IsMeat(ThingDef def)
    {
        return def.IsMeat;
    }
    public static bool IsFoodProduce(ThingDef def)
    {
        return def.IsWithinCategory(ThingCategoryDefOf.PlantFoodRaw);
    }
    public static bool IsAnimalProduct(ThingDef def)
    {
        return def.IsAnimalProduct;
    }
    public static bool IsWeapon(ThingDef def)
    {
        return def.IsWeapon;
    }
    public static bool IsMeleeWeapon(ThingDef def)
    {
        return def.IsMeleeWeapon;
    }
    public static bool IsRangedWeapon(ThingDef def)
    {
        return def.IsRangedWeapon;
    }
    public static bool IsGrenade(ThingDef def)
    {
        return def.IsWithinCategory(DefDatabase<ThingCategoryDef>.GetNamed("Grenades"));
    }
    public static bool IsApparel(ThingDef def)
    {
        return def.IsApparel;
    }
    public static bool IsBuilding(ThingDef def)
    {
        return def.category == Verse.ThingCategory.Building;
    }
    public static bool IsCreature(ThingDef def)
    {
        return def.race is not null;
    }
    public static bool IsAnimal(ThingDef def)
    {
        return def.race?.Animal == true;
    }
    public static bool IsAnimalDomestic(ThingDef def)
    {
        return def.race?.wildness == 0;
    }
    public static bool IsAnimalWild(ThingDef def)
    {
        return def.race?.wildness > 0;
    }
    public static bool IsCreatureMech(ThingDef def)
    {
        return def.race?.IsMechanoid == true;
    }
    public static bool IsPlant(ThingDef def)
    {
        return def.IsPlant;
    }
    public static bool IsPlantSowable(ThingDef def)
    {
        return def.plant?.Sowable == true;
    }
    public static bool IsPlantWild(ThingDef def)
    {
        return !IsPlantSowable(def);
    }
    public static bool IsPlantProduceYeilding(ThingDef def)
    {
        //return def.plant?.humanFoodPlant == true;
        return def.plant?.harvestedThingDef?.statBases?.Any(statBase =>
            statBase?.stat == StatDefOf.Nutrition && statBase.value > 0
        ) == true;
    }
    public static bool IsPlantTree(ThingDef def)
    {
        return def.plant?.IsTree == true;
    }
}