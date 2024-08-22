using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

public static class UnityEngine_Rect
{
    public static Rect CutFromX(this Rect rect, float x, float? amount = null)
    {
        return new Rect(x, rect.y, amount ?? rect.xMax - x, rect.height);
    }
    public static Rect CutFromX(this Rect rect, ref float x, float? amount = null)
    {
        var result = rect.CutFromX(x, amount);

        x = result.xMax;

        return result;
    }
    public static Rect CutFromY(this Rect rect, float y, float? amount = null)
    {
        return new Rect(rect.x, y, rect.width, amount ?? rect.yMax - y);
    }
    public static Rect CutFromY(this Rect rect, ref float y, float? amount = null)
    {
        var result = rect.CutFromY(y, amount);

        y = result.yMax;

        return result;
    }
}

public static class Verse_ThingCategoryDef
{
    // What about duplicates?
    public static IEnumerable<ThingDef> AllThingDefs(this ThingCategoryDef categoryDef)
    {
        foreach (var thingDef in categoryDef.childThingDefs)
        {
            yield return thingDef;
        }

        foreach (var childCat in categoryDef.childCategories)
        {
            foreach (var thingDef in childCat.AllThingDefs())
            {
                yield return thingDef;
            }
        }
    }
}