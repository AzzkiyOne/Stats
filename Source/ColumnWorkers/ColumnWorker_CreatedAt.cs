using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    protected override IWidget GetTableCellContent(
        IEnumerable<ThingDef> things,
        ThingRec thing
    )
    {
        var icons = new List<IWidget>();

        foreach (var thingDef in things.OrderBy(def => def.label))
        {
            void onDrawIcon(ref Rect rect)
            {
                Widgets.DrawHighlightIfMouseover(rect);

                if (Widgets.ButtonInvisible(rect))
                {
                    Widget_DefInfoDialog.Draw(thingDef);
                }
            }

            IWidget
            icon = new Widget_Icon_Thing(thingDef);
            icon = new WidgetComp_Size_Abs(icon, Text.LineHeight);
            icon = new WidgetComp_Tooltip(icon, thingDef.description);
            icon = new WidgetComp_Generic(icon, onDrawIcon);

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
