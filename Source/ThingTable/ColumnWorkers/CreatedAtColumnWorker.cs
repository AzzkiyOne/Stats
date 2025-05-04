using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;
using Stats.ThingTable.Defs;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ThingTable.ColumnWorkers;

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
    private CreatedAtColumnWorker() : base(TableColumnCellStyle.String)
    {
    }
    public static CreatedAtColumnWorker Make(ColumnDef _) => new();
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
                .ToButtonSubtle(
                    openDefInfoDialog,
                    $"<i>{thingDef.LabelCap}</i>\n\n{thingDef.description}"
                );

            icons.Add(icon);
        }

        return new HorizontalContainer(icons, Globals.GUI.PadSm);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget()
    {
        var craftingBenches = DefDatabase<ThingDef>.AllDefsListForReading.Where(
            thingDef => thingDef.IsWorkTable
        );

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
