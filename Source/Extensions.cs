using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal static class UnityEngine_Rect
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
internal static class VerbList
{
    public static VerbProperties? Primary(this List<VerbProperties> verbs)
    {
        return verbs.FirstOrFallback(v => v?.isPrimary == true);
    }
}
