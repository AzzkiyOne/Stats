using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Stats.Widgets.Comps;
using Stats.Widgets.Comps.Size.Constraints;
using Stats.Widgets.Containers;
using Stats.Widgets.Misc;
using Stats.Widgets.Table.Filters.Widgets;
using Verse;

namespace Stats.ColumnWorkers;

public class CreatedAtColumnWorker
    : ColumnWorker<IEnumerable<ThingDef>>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.String;
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
    protected override bool ShouldShowValue(IEnumerable<ThingDef> things)
    {
        return things.Count() > 0;
    }
    protected override IWidget GetTableCellContent(
        IEnumerable<ThingDef> things,
        ThingAlike thing
    )
    {
        var icons = new List<IWidget>();

        foreach (var thingDef in things.OrderBy(def => def.label))
        {
            void handleIconClick()
            {
                DefInfoDialogWidget.Draw(thingDef);
            }

            IWidget icon = new ThingIconWidget(thingDef);
            new TooltipWidgetComp(ref icon, $"<i>{thingDef.LabelCap}</i>\n\n{thingDef.description}");
            new TextureHoverWidgetComp(ref icon, TexUI.HighlightTex);
            new OnClickWidgetComp(ref icon, handleIconClick);

            icons.Add(icon);
        }

        return new HorizontalContainerWidget(icons, 5f);
    }
    public override IFilterWidget GetFilterWidget()
    {
        var craftingBenches = DefDatabase<ThingDef>.AllDefsListForReading.Where(def => def.IsWorkTable);

        return new EnumerableFilterWidget<ThingDef>(
            GetValueCached,
            craftingBenches,
            MakeFilterOptionWidget
        );
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValueCached(thing1).Count().CompareTo(GetValueCached(thing2).Count());
    }
    private static IWidget MakeFilterOptionWidget(ThingDef thingDef)
    {
        var icon = new ThingIconWidget(thingDef);

        IWidget label = new LabelWidget(thingDef.LabelCap);
        new WidgetComp_Width_Rel(ref label, 1f);

        return new HorizontalContainerWidget([icon, label], 5f, true);
    }
}
