using UnityEngine;

namespace Stats.Widgets.Extensions.Color;

public sealed class ColorWidgetExtension : WidgetExtension
{
    public UnityEngine.Color Color { get; set; }
    internal ColorWidgetExtension(Widget widget, UnityEngine.Color color) : base(widget)
    {
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
