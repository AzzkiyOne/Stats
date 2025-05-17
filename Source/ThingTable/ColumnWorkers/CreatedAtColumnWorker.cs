using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats.ThingTable;

public sealed class CreatedAtColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Dictionary<ThingDef, HashSet<ThingDef>> ThingCraftingBenches = [];
    static CreatedAtColumnWorker()
    {
        foreach (var recipeDef in DefDatabase<RecipeDef>.AllDefs)
        {
            // See Verse.ThingDef.SpecialDisplayStats()
            if (recipeDef is { products.Count: 1, IsSurgery: false })
            {
                var producedThingDef = recipeDef.products[0]?.thingDef;

                if (producedThingDef == null)
                {
                    continue;
                }

                if (recipeDef.recipeUsers?.Count > 0)
                {
                    var craftingBenchesEntryExists = ThingCraftingBenches.TryGetValue(producedThingDef, out var craftingBenches);

                    if (craftingBenchesEntryExists)
                    {
                        craftingBenches.AddRange(recipeDef.recipeUsers);
                    }
                    else
                    {
                        ThingCraftingBenches[producedThingDef] = [.. recipeDef.recipeUsers];
                    }
                }
            }
        }
    }
    private static readonly Func<ThingAlike, HashSet<ThingDef>> GetThingCraftingBenchesDefs =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var thingCanBeCrafted = ThingCraftingBenches.TryGetValue(thing.Def, out var craftingBenchesDefs);

        if (thingCanBeCrafted)
        {
            return craftingBenchesDefs;
        }

        return [];
    });
    public CreatedAtColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var thingDefs = GetThingCraftingBenchesDefs(thing);

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
        var craftingBenchesDefs = tableRecords
            .SelectMany(GetThingCraftingBenchesDefs)
            .Distinct()
            .OrderBy(thingDef => thingDef.label);

        return new ManyToManyFilter<ThingAlike, ThingDef>(
            GetThingCraftingBenchesDefs,
            craftingBenchesDefs,
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
        var craftingBechesCount1 = GetThingCraftingBenchesDefs(thing1).Count;
        var craftingBechesCount2 = GetThingCraftingBenchesDefs(thing2).Count;

        return craftingBechesCount1.CompareTo(craftingBechesCount2);
    }
}
