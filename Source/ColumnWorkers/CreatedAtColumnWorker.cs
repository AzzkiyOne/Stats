using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;
using Verse;

namespace Stats.ColumnWorkers;

public sealed class CreatedAtColumnWorker : ColumnWorker<IEnumerable<ThingDef>>
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    protected override IEnumerable<ThingDef> GetValue(ThingAlike thing)
    {
        var things = new HashSet<ThingDef>();

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
                    things.Add(recipeUser);
                }
            }
        }

        return things;
    }
    protected override bool ShouldShowValue(IEnumerable<ThingDef> thingDefs)
    {
        return thingDefs.Count() > 0;
    }
    protected override Widget GetTableCellContent(
        IEnumerable<ThingDef> thingDefs,
        ThingAlike thing
    )
    {
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
        var craftingBenches = DefDatabase<ThingDef>.AllDefsListForReading.Where(thingDef => thingDef.IsWorkTable);

        return new EnumerableFilter<ThingDef>(
            GetValueCached,
            craftingBenches,
            ThingDefToFilterOptionWidget
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).Count().CompareTo(GetValueCached(thing2).Count());
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
}
