using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats;

public class ColumnWorker_CreatedAt
    : ColumnWorker<IEnumerable<ThingDef>>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.String;
    protected override IEnumerable<ThingDef> GetValue(ThingRec thing)
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
        ThingRec thing
    )
    {
        var icons = new List<IWidget>();

        foreach (var thingDef in things.OrderBy(def => def.label))
        {
            void handleIconClick()
            {
                Widget_DefInfoDialog.Draw(thingDef);
            }

            IWidget icon = new Widget_Icon_Thing(thingDef);
            new WidgetComp_Tooltip(ref icon, $"<i>{thingDef.LabelCap}</i>\n\n{thingDef.description}");
            new WidgetComp_Bg_Tex_Hover(ref icon, TexUI.HighlightTex);
            new WidgetComp_OnClick(ref icon, handleIconClick);

            icons.Add(icon);
        }

        return new Widget_Container_Hor(icons, 5f);
    }
    public override IWidget_FilterInput GetFilterWidget()
    {
        return new Widget_FilterInput_Str(
            new(thing => string.Join(",", GetValueCached(thing).Select(thing => thing.LabelCap)))
        );
    }
    public override int Compare(ThingRec thing1, ThingRec thing2)
    {
        return GetValueCached(thing1).Count().CompareTo(GetValueCached(thing2).Count());
    }
}
