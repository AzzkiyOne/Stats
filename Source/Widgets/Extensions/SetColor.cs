using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class SetColor
    : WidgetExtension
{
    public Color Color { get; set; }
    internal SetColor(IWidget widget, Color color)
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

public static partial class WidgetAPI
{
    public static SetColor Color(this IWidget widget, Color color)
    {
        return new(widget, color);
    }
}
