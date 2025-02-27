using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats;

public class ColumnWorker_CreatedAt : ColumnWorker<IEnumerable<ThingDef>>
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
    protected override ICellWidget ValueToCellWidget(
        IEnumerable<ThingDef> things,
        ThingRec thing
    )
    {
        return new CellWidget_Things(things);
    }
    public override IFilterWidget GetFilterWidget()
    {
        return new FilterWidget_Str(thing => string.Join(", ", GetValue(thing).Select(thing => thing.LabelCap)));
    }
    public override int Compare(ThingRec thing1, ThingRec thing2)
    {
        return GetValue(thing1).Count().CompareTo(GetValue(thing2).Count());
    }
}
