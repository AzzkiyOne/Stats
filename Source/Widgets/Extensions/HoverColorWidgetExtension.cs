using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class HoverColorWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly Color Color;
    internal HoverColorWidgetExtension(Widget widget, Color color)
    {
        Widget = widget;
        Color = color;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Mouse.IsOver(rect))
        {
            var origGUIColor = GUI.color;
            GUI.color = Color;
            Widget.Draw(rect, containerSize);
            GUI.color = origGUIColor;
        }
        else
        {
            Widget.Draw(rect, containerSize);
        }
    }
}
