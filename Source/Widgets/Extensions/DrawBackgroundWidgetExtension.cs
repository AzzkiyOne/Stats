using System;
using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class DrawBackgroundWidgetExtension : WidgetExtension
{
    private readonly Action<Rect> DrawBackground;
    internal DrawBackgroundWidgetExtension(
        Widget widget,
        Action<Rect>
        drawBackground
    ) : base(widget)
    {
        DrawBackground = drawBackground;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        if (Event.current.type == EventType.Repaint)
        {
            DrawBackground(rect);
        }

        Widget.Draw(rect, containerSize);
    }
}
