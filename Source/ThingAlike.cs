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

                    Add(new ThingAlike(ThingDefTable_Columns.list.Count, thingDef, label, stuffDef));
                }
            }
            else
            {
                Add(new ThingAlike(ThingDefTable_Columns.list.Count, thingDef, thingDef.LabelCap));
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
public class ThingAlike :
    Dictionary<ThingDefTable_Column, IGenTable_Cell>,
    IGenTable_Row<ThingDefTable_Column>
{
    public readonly string label;
    public readonly ThingDef def;
    public readonly ThingDef? stuff;

    public ThingAlike(
        int size,
        ThingDef def,
        string label,
        ThingDef? stuff = null
    ) : base(size)
    {
        this.label = label;
        this.def = def;
        this.stuff = stuff;
    }

    public IGenTable_Cell GetCell(ThingDefTable_Column column)
    {
        var containsValue = TryGetValue(column, out var value);

        if (!containsValue)
        {
            var newValue = column.GetCellData(this);

            this[column] = newValue;

            return newValue;
        }

        return value;
    }
}
