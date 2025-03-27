using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal static class UnityEngine_Rect
{
    public static Rect CutByX(ref this Rect rect, float amount)
    {
        var result = new Rect(rect.x, rect.y, amount, rect.height);
        // Changing "xMin" also auto corrects width. Changing "x" don't.
        rect.xMin += amount;

        return result;
    }
    public static Rect CutByY(ref this Rect rect, float amount)
    {
        var result = new Rect(rect.x, rect.y, rect.width, amount);
        // Changing "yMin" also auto corrects height. Changing "y" don't.
        rect.yMin += amount;

        return result;
    }
    public static void PadLeft(ref this Rect rect, float amount)
    {
        rect.xMin += amount;
    }
    public static void ContractBy(ref this Rect rect, WidgetStyle.BoxOffset offset)
    {
        rect.xMin += offset.Left;
        rect.yMin += offset.Top;
        rect.xMax -= offset.Right;
        rect.yMax -= offset.Bottom;
    }
}

internal static class VerbList
{
    public static VerbProperties? Primary(this List<VerbProperties> verbs)
    {
        return verbs.FirstOrFallback(v => v?.isPrimary == true);
    }
}
