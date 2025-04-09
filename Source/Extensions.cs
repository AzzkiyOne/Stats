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
}

public static class Ext_VerbList
{
    public static VerbProperties? Primary(this List<VerbProperties> verbs)
    {
        return verbs.FirstOrFallback(v => v?.isPrimary == true);
    }
}

public static class Widget_Ext
{
    // Doesn't include widget's "own" size.
    public static Vector2 GetFixedSize(this IWidget widget)
    {
        return widget.GetSize(Vector2.zero);
    }
    public static void DrawIn(this IWidget widget, Rect rect)
    {
        rect.size = widget.GetSize(rect.size);
        widget.Draw(rect, rect.size);
    }
}

public static class String_Ext
{
    public static Color ToUniqueColorRGB(this string str)
    {
        // I don't think that this hash is very reliable.
        var hash = (uint)str.GetHashCode();
        var r = ((hash & 0xFF000000) >> 24) / 255f;
        var g = ((hash & 0x00FF0000) >> 16) / 255f;
        var b = ((hash & 0x0000FF00) >> 8) / 255f;

        return new Color(r, g, b);
    }
}
