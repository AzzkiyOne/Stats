using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers;

public sealed class CreatedAtColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    private static readonly Func<ThingAlike, HashSet<ThingDef>> GetThingCraftBenches = FunctionExtensions.Memoized(
        (ThingAlike thing) =>
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
        }
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var thingDefs = GetThingCraftBenches(thing);

        if (thingDefs.Count == 0)
        {
            return null;
        }

        var icons = new List<Widget>();

        foreach (var thingDef in thingDefs.OrderBy(thingDef => thingDef.label))
        {
            void openDefInfoDialog()
            {
                DefInfoDialog.Draw(thingDef);
            }

            Widget icon = new ThingIcon(thingDef)
                .ToButtonSubtle(
                    openDefInfoDialog,
                    $"<i>{thingDef.LabelCap}</i>\n\n{thingDef.description}"
                );

            icons.Add(icon);
        }

        return new HorizontalContainer(icons, Globals.GUI.PadSm);
    }
    public override FilterWidget GetFilterWidget()
    {
        var craftingBenches = DefDatabase<ThingDef>.AllDefsListForReading.Where(
            thingDef => thingDef.IsWorkTable
        );

        return new ManyToManyOptionsFilter<ThingDef>(
            GetThingCraftBenches,
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
        var craftBechesCount1 = GetThingCraftBenches(thing1).Count;
        var craftBechesCount2 = GetThingCraftBenches(thing2).Count;

        return craftBechesCount1.CompareTo(craftBechesCount2);
    }
}
