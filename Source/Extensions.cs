using System;
using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public static class Ext_UnityEngine_Rect
{
    public static Rect CutByX(ref this Rect rect, float amount)
    {
        var result = rect with { width = amount };
        // Changing "xMin" also auto corrects width. Changing "x" doesn't.
        rect.xMin += amount;

        return result;
    }
    public static Rect CutByY(ref this Rect rect, float amount)
    {
        var result = rect with { height = amount };
        // Changing "yMin" also auto corrects height. Changing "y" doesn't.
        rect.yMin += amount;

        return result;
    }
}

public static class Ext_VerbList
{
    public static VerbProperties? Primary(this List<VerbProperties> verbs)
    {
        return verbs.FirstOrFallback(static v => v?.isPrimary == true);
    }
}

public static class Ext_Widget
{
    // Doesn't include widget's "own" size.
    public static Vector2 GetFixedSize(this IWidget widget)
    {
        return widget.GetSize(Vector2.zero);
    }
    public static void DrawIn(this IWidget widget, Rect rect)
    {
        var size = widget.GetSize(rect.size);
        widget.Draw(rect with { size = size }, rect.size);
    }
}

public static class Ext_String
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
    public static bool Contains(this string str, string substr, StringComparison comp)
    {
        return str.IndexOf(substr, comp) >= 0;
    }
}
