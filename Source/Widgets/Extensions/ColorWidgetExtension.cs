using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class ColorWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    public Color Color { get; set; }
    internal ColorWidgetExtension(Widget widget, Color color)
    {
        Widget = widget;
        Color = color;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        var origGUIColor = GUI.color;
        GUI.color = Color;

        Widget.Draw(rect, containerSize);

        GUI.color = origGUIColor;
    }
}
