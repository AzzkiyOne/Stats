using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

public static class Ext_UnityEngine_Rect
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
}

public static class Ext_VerbList
{
    public static VerbProperties? Primary(this List<VerbProperties> verbs)
    {
        return verbs.FirstOrFallback(v => v?.isPrimary == true);
    }
}

public static class Ext_Widget
{
    public static void DrawIn(this IWidget widget, Rect container)
    {
        widget.Draw(
            new Rect(container.position, widget.GetSize(container.size)),
            container.size
        );
    }
}
