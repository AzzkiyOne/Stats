using UnityEngine;

namespace Stats.Widgets.Extensions.Overflow;

public sealed class HideOverflowWidgetExtension : WidgetExtension
{
    internal HideOverflowWidgetExtension(Widget widget) : base(widget)
    {
        Resize();
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
