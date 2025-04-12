using UnityEngine;

namespace Stats;

public class WidgetComp_Color
    : WidgetComp
{
    public Color Color { get; set; }
    public WidgetComp_Color(ref IWidget widget, Color color)
        : base(ref widget)
    {
        Color = color;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        var origGUIColor = GUI.color;
        GUI.color = Color;

        Widget.Draw(rect, containerSize);

        GUI.color = origGUIColor;
    }
}
