using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Stats;

public static class UnityEngineRectExtensions
{
    internal static Rect CutByX(ref this Rect rect, float amount)
    {
        var result = rect with { width = amount };
        // Changing "xMin" also auto corrects width. Changing "x" doesn't.
        rect.xMin += amount;

        return result;
    }
    internal static Rect CutByY(ref this Rect rect, float amount)
    {
        var result = rect with { height = amount };
        // Changing "yMin" also auto corrects height. Changing "y" doesn't.
        rect.yMin += amount;

        return result;
    }
}

public static class VerseVerbPropertiesListExtensions
{
    public static VerbProperties? Primary(this List<VerbProperties> verbs)
    {
        return verbs.FirstOrFallback(static verb => verb?.isPrimary == true);
    }
}

public static class StringExtensions
{
    internal static Color ToUniqueColorRGB(this string str)
    {
        // I don't think that this hash is very reliable.
        var hash = (uint)str.GetHashCode();
        var r = ((hash & 0xFF000000) >> 24) / 255f;
        var g = ((hash & 0x00FF0000) >> 16) / 255f;
        var b = ((hash & 0x0000FF00) >> 8) / 255f;

        return new Color(r, g, b);
    }
    internal static bool Contains(this string str, string substr, StringComparison comp)
    {
        return str.IndexOf(substr, comp) >= 0;
    }
}

public static class UnityEngineColorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color AdjustedForGUIOpacity(this Color color)
    {
        color.a *= Globals.GUI.Opacity;

        return color;
    }
}

public static class FunctionExtensions
{
    // Memoize every function call.
    public static Func<TArg, TRes> Memoized<TArg, TRes>(this Func<TArg, TRes> function)
    {
        var cache = new Dictionary<TArg, TRes>();

        return (TArg arg) =>
        {
            var resultIsCached = cache.TryGetValue(arg, out var result);

            if (resultIsCached)
            {
                return result;
            }

            return cache[arg] = function(arg);
        };
    }
}

public static class SystemSingleExtensions
{
    public static decimal ToDecimal(this float value, int digits)
    {
        // "When you convert float or double to decimal, the source value is converted
        // to decimal representation and rounded to the nearest number after
        // the 28th decimal place if necessary."
        //
        // Key word is "if".
        return (decimal)Math.Round(value, digits);
    }
}
