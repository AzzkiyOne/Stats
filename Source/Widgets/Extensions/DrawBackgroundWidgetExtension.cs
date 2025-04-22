using System;
using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class DrawBackgroundWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly Action<Rect> _DrawBackground;
    internal DrawBackgroundWidgetExtension(
        Widget widget,
        Action<Rect>
        drawBackground
    )
    {
        Widget = widget;
        _DrawBackground = drawBackground;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        _DrawBackground(rect);

        Widget.Draw(rect, containerSize);
    }
}
