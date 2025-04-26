using System;
using UnityEngine;

namespace Stats.Widgets.Extensions.Background;

public sealed class DrawBackgroundWidgetExtension : WidgetExtension
{
    private readonly Action<Rect> _DrawBackground;
    internal DrawBackgroundWidgetExtension(
        Widget widget,
        Action<Rect>
        drawBackground
    ) : base(widget)
    {
        _DrawBackground = drawBackground;

        Resize();
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        _DrawBackground(rect);

        Widget.Draw(rect, containerSize);
    }
}
