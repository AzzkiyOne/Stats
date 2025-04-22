using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class ColorWidgetExtension
    : WidgetExtension
{
    public Color Color { get; set; }
    internal ColorWidgetExtension(IWidget widget, Color color)
        : base(widget)
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
