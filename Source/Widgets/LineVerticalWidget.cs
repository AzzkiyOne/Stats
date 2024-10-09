using UnityEngine;
using Verse;

namespace Stats;

internal static class LineVerticalWidget
{
    public static void Draw(float x, float y, float length, Color color)
    {
        GUI.color = color;
        Widgets.DrawLineVertical(x, y, length);
        GUI.color = Color.white;
    }
}
