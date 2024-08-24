using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats;

static class ThingAlikes
{
    public static List<ThingAlike> list = [];
    public static Dictionary<ThingCategoryDef, List<ThingAlike>> byCategory = [];
    static ThingAlikes()
    {
        foreach (var thingCatDef in DefDatabase<ThingCategoryDef>.AllDefs)
        {
            byCategory[thingCatDef] = [];
        }

        foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
        {
            if (
                thingDef.IsBlueprint
                || thingDef.IsFrame
                || thingDef.isUnfinishedThing
                || thingDef.IsCorpse
                || (
                    thingDef.category != ThingCategory.Pawn
                    && thingDef.category != ThingCategory.Item
                    && thingDef.category != ThingCategory.Building
                    && thingDef.category != ThingCategory.Plant
                )
            )
            {
                continue;
            }

            if (thingDef.MadeFromStuff)
            {
                var allowedStuffs = GenStuff.AllowedStuffsFor(thingDef);

                foreach (var stuffDef in allowedStuffs)
                {
                    //var label = GenLabel.ThingLabel(thingDef, stuffDef, 0).CapitalizeFirst();
                    var label = thingDef.LabelCap + " (" + stuffDef.LabelCap + ")";

                    Add(new ThingAlike(thingDef, label, stuffDef));
                }
            }
            else
            {
                Add(new ThingAlike(thingDef, thingDef.LabelCap));
            }
        }
    }
    private static void Add(ThingAlike thingAlike)
    {
        list.Add(thingAlike);

        if (thingAlike.def.thingCategories is null)
        {
            return;
        }

        foreach (var thingCatDef in thingAlike.def.thingCategories)
        {
            byCategory[thingCatDef].Add(thingAlike);
        }
    }
}

// Implement GetHashCode (and Equals) just in case?
public class ThingAlike(
    ThingDef def,
    string label,
    ThingDef? stuff = null
)
{
    public readonly string label = label;
    public readonly ThingDef def = def;
    public readonly ThingDef? stuff = stuff;
}
