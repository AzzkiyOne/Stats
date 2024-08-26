using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

public static class UnityEngine_Rect
{
    public static Rect CutFromX(this Rect rect, ref float x, float? amount = null)
    {
        var result = new Rect(x, rect.y, amount ?? rect.xMax - x, rect.height);

        x = result.xMax;

        return result;
    }
    public static Rect CutFromY(this Rect rect, ref float y, float? amount = null)
    {
        var result = new Rect(rect.x, y, rect.width, amount ?? rect.yMax - y);

        y = result.yMax;

        return result;
    }
}

public static class Verse_ThingCategoryDef
{
    // What about duplicates?
    public static IEnumerable<ThingAlike> AllThingAlikes(this ThingCategoryDef categoryDef)
    {
        foreach (var thingAlike in ThingAlikes.byCategory[categoryDef])
        {
            yield return thingAlike;
        }

        foreach (var childCat in categoryDef.childCategories)
        {
            foreach (var thingAlike in childCat.AllThingAlikes())
            {
                yield return thingAlike;
            }
        }
    }
}
