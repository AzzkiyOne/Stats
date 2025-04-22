using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class HideOverflowWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    internal HideOverflowWidgetExtension(Widget widget)
    {
        Widget = widget;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        GUI.BeginClip(rect);

        rect.x = 0f;
        rect.y = 0f;
        Widget.Draw(rect, containerSize);

        GUI.EndClip();
    }
}
