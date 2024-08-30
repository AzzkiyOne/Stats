using System;
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
        var columnDefs = DefDatabase<ColumnDef>.AllDefsListForReading;

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

                    Add(new ThingAlike(columnDefs.Count, thingDef, label, stuffDef));
                }
            }
            else
            {
                Add(new ThingAlike(columnDefs.Count, thingDef, thingDef.LabelCap));
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
    Dictionary<ColumnDef, Cell?>,
    IGenTable_Row<ColumnDef>
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

    public Cell? GetCell(ColumnDef column)
    {
        var containsValue = TryGetValue(column, out var value);

        if (!containsValue)
        {
            Cell? newValue;

            try
            {
                newValue = column.CreateCell(this);
            }
            catch (Exception ex)
            {
                newValue = new ExCell(ColumnDefOf.ExCellColumn, ex);
            }

            this[column] = newValue;

            return newValue;
        }

        return value;
    }
}
