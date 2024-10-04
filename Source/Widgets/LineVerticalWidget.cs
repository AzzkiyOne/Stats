using UnityEngine;
using Verse;

namespace Stats;

internal static class LineVerticalWidget
{
    public static void Draw(float x, float y, float length, Color color)
    {
        using (new ColorCtx(color))
        {
            Widgets.DrawLineVertical(x, y, length);
        }
    }
}
