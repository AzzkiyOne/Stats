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
    /*
    
    To explain how did i came to this solution, let's take some ExampleThing and it's ExampleStat.

    Disclaimer: all float values are for demonstration purposes only.

    float exStatValue = ExampleThing.ExampleStat;// 12.4001

    If we open ExampleThing's info dialog, we'll see that it's ExampleStat value is displayed as "12.4".
    That is because RW uses ToString("F1") to turn float into a string.

    Naturally, i do the same thing to display the value in the table cell.

    Now let's add filter widgets.

    Number-filter widget has a text input field, where a user types a number as text.
    This text is then parsed to float and we get a value close to what the user has typed into the 
    input field.

    Now if a user wants to have only rows where ExampleThing.ExampleStat == 12.4 be displayed, he types 
    "12.4" and parsed float will be 12.4000000115 for example. 12.4000000115 != 12.4001 and the user 
    sees incorrect results.

    Let's examine possible solutions to this problem.

    1. "Parse back" displayed value.

    float exStatValue = ExampleThing.ExampleStat;
    string exStatDisplayedValue = exStatValue.ToString("F1");
    exStatValue = float.Parse(exStatDisplayedValue);

    The assumption here is that if we have two floats that were produced the same way from the same 
    source (cell's text/input field text), we can safely compare them.

    But because of float imprecision, two different strings can be parsed to the same float value. And if 
    we have to "parse back" the value from a string, then why not to parse it to some more precise and 
    easy to use type?

    2. Parse to decimal.

    2.1 (decimal)Math.Round(value, digits);

    First, the value get's converted to double, since there is no Math methods that work directly with 
    floats. Then it gets rounded to the desired precision (which we can't obtain from StatWorker). Lastly,
    it get's converted to decimal, which can cause another rounding ("double -> decimal might result in 
    data loss or throw an exception").

    2.2 Math.Round((decimal)value, digits);

    First, the values is converted to decimal, which can cause it to be rounded. Then, it get's rounded
    again (if we'll even get to this point and won't catch an exception).

    2.3 decimal.Parse(value.ToString(format));

    This is inefficient and hacky, but it is as precise as it gets. We get the exact value as it is 
    displayed in thing's info card.

    */
    public static decimal ToDecimal(this float value, string format = "F0")
    {
        return decimal.Parse(value.ToString(format));
    }
}
