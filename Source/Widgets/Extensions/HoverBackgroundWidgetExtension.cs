using UnityEngine;
using Verse;

namespace Stats.Widgets.Extensions;

public sealed class HoverBackgroundWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly Texture2D Texture;
    internal HoverBackgroundWidgetExtension(Widget widget, Texture2D texture)
    {
        Widget = widget;
        Texture = texture;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if
        (
            Event.current.type == EventType.Repaint
            &&
            Mouse.IsOver(rect)
        )
        {
            GUI.DrawTexture(rect, Texture);
        }

        Widget.Draw(rect, containerSize);
    }
}
