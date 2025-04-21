using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class HideOverflow
    : WidgetExtension
{
    internal HideOverflow(IWidget widget)
        : base(widget)
    {
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        GUI.BeginClip(rect);

        rect.x = 0f;
        rect.y = 0f;
        Widget.Draw(rect, containerSize);

        GUI.EndClip();
    }
}
