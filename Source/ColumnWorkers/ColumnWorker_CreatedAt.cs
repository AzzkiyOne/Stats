using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats;

public class ColumnWorker_CreatedAt
    : ColumnWorker<IEnumerable<ThingDef>>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.String;
    public override IEnumerable<ThingDef> GetValue(ThingRec thing)
    {
        var things = new HashSet<ThingDef>();

        foreach (var recipe in DefDatabase<RecipeDef>.AllDefs)
        {
            if (
                recipe is { products.Count: 1, IsSurgery: false }
                && recipe.products.First().thingDef == thing.Def
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
    protected override Widget GetTableCellContent(
        IEnumerable<ThingDef> things,
        ThingRec thing
    )
    {
        var icons = new List<Widget>();

        foreach (var thingDef in things.OrderBy(def => def.label))
        {
            var iconStyle = new WidgetStyle()
            {
                Width = Text.LineHeight,
                Height = Text.LineHeight,
                Background = (borderBox, _) =>
                {
                    Widgets.DrawHighlightIfMouseover(borderBox);

                    if (Widgets.ButtonInvisible(borderBox))
                    {
                        Widget_DefInfoDialog.Draw(thingDef);
                    }
                },
            };
            var icon = new Widget_Icon_Thing(thingDef, style: iconStyle)
            {
                Tooltip = thingDef.description,
            };

            icons.Add(icon);
        }

        return new Widget_Container_Hor(icons, 5f);
    }
    public override IWidget_FilterInput GetFilterWidget()
    {
        return new Widget_FilterInput_Str(thing => string.Join(", ", GetValue(thing).Select(thing => thing.LabelCap)));
    }
    public override int Compare(ThingRec thing1, ThingRec thing2)
    {
        return GetValue(thing1).Count().CompareTo(GetValue(thing2).Count());
    }
}
