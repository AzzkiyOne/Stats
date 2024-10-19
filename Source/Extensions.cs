using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal static class UnityEngine_Rect
{
    public static Rect CutFromX(this Rect rect, ref float x, float amount)
    {
        var result = new Rect(x, rect.y, amount, rect.height);
        x = result.xMax;

        return result;
    }
    public static Rect CutFromX(this Rect rect, float x)
    {
        return new Rect(x, rect.y, Math.Max(rect.xMax - x, 0f), rect.height);
    }
    public static Rect CutFromY(this Rect rect, ref float y, float amount)
    {
        var result = new Rect(rect.x, y, rect.width, amount);
        y = result.yMax;

        return result;
    }
    public static Rect CutFromY(this Rect rect, float y)
    {
        return new Rect(rect.x, y, rect.width, Math.Max(rect.yMax - y, 0f));
    }
    public static Rect CutByX(ref this Rect rect, float amount)
    {
        var result = new Rect(rect.x, rect.y, amount, rect.height);
        // Changing "xMin" also auto corrects width. Changing "x" don't.
        rect.xMin = rect.xMin + amount;

        return result;
    }
    public static Rect CutByY(ref this Rect rect, float amount)
    {
        var result = new Rect(rect.x, rect.y, rect.width, amount);
        // Changing "yMin" also auto corrects height. Changing "y" don't.
        rect.yMin = rect.yMin + amount;

        return result;
    }
    public static void PadLeft(ref this Rect rect, float amount)
    {
        rect.xMin = rect.xMin + amount;
    }
}
internal static class VerbList
{
    public static VerbProperties? Primary(this List<VerbProperties> verbs)
    {
        return verbs.FirstOrFallback(v => v?.isPrimary == true);
    }
}
