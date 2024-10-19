using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats;

public class ColumnWorker_CreatedAt : ColumnWorker<ICellWidget<List<ThingAlike>>>
{
    protected override ICellWidget<List<ThingAlike>>? CreateCell(ThingRec thing)
    {
        var things = new HashSet<ThingAlike>();

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

        if (things.Count > 0)
        {
            return new CellWidget_Things(things.ToList());
        }

        return null;
    }
}
