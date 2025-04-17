using UnityEngine;
using Verse;

namespace Stats;

internal static class Widget_LineVertical
{
    public static void Draw(float x, float y, float length, Color color)
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        var origGUIColor = GUI.color;
        GUI.color = color;
        Widgets.DrawLineVertical(x, y, length);
        GUI.color = origGUIColor;
    }
}
