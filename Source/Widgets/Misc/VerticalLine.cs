using UnityEngine;

namespace Stats.Widgets.Misc;

internal static class VerticalLine
{
    public static void Draw(float x, float y, float length, Color color)
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        var origGUIColor = GUI.color;
        GUI.color = color;
        Verse.Widgets.DrawLineVertical(x, y, length);
        GUI.color = origGUIColor;
    }
}
