using System;
using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class DrawBackgroundWidgetExtension
    : WidgetExtension,
      IWidget
{
    private readonly Action<Rect> _DrawBackground;
    internal DrawBackgroundWidgetExtension(
        IWidget widget,
        Action<Rect>
        drawBackground
    ) : base(widget)
    {
        _DrawBackground = drawBackground;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        _DrawBackground(rect);

        Widget.Draw(rect, containerSize);
    }
}
