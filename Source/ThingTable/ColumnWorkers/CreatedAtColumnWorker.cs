﻿using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class CreatedAtColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Func<ThingAlike, HashSet<ThingDef>> GetThingCraftingBenches =
        FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            var craftBenchesDefs = new HashSet<ThingDef>();

            foreach (var recipe in DefDatabase<RecipeDef>.AllDefs)
            {
                if
                (
                    recipe is { products.Count: 1, IsSurgery: false }
                    &&
                    recipe.products.First().thingDef == thing.Def
                )
                {
                    foreach (var recipeUser in recipe.AllRecipeUsers)
                    {
                        craftBenchesDefs.Add(recipeUser);
                    }
                }
            }

            return craftBenchesDefs;
        });
    public CreatedAtColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var thingDefs = GetThingCraftingBenches(thing);

        if (thingDefs.Count == 0)
        {
            return null;
        }

        var icons = new List<Widget>();

        foreach (var thingDef in thingDefs.OrderBy(thingDef => thingDef.label))
        {
            void openDefInfoDialog()
            {
                Draw.DefInfoDialog(thingDef);
            }

            Widget icon = new ThingIcon(thingDef)
                .ToButtonGhostly(
                    openDefInfoDialog,
                    $"<i>{thingDef.LabelCap}</i>\n\n{thingDef.description}"
                );

            icons.Add(icon);
        }

        return new HorizontalContainer(icons, Globals.GUI.PadSm);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var craftingBenches = tableRecords
            .SelectMany(GetThingCraftingBenches)
            .Distinct()
            .OrderBy(thingDef => thingDef.label);

        return new ManyToManyFilter<ThingAlike, ThingDef>(
            GetThingCraftingBenches,
            craftingBenches,
            ThingDefToFilterOptionWidget
        );
    }
    private static Widget ThingDefToFilterOptionWidget(ThingDef thingDef)
    {
        return new HorizontalContainer(
            [
                new ThingIcon(thingDef),
                new Label(thingDef.LabelCap).WidthRel(1f)
            ],
            Globals.GUI.PadSm,
            true
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        var craftingBechesCount1 = GetThingCraftingBenches(thing1).Count;
        var craftingBechesCount2 = GetThingCraftingBenches(thing2).Count;

        return craftingBechesCount1.CompareTo(craftingBechesCount2);
    }
}
