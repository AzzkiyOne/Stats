using System;
using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class DrawBackground
    : WidgetExtension,
      IWidget
{
    private readonly Action<Rect> _DrawBackground;
    internal DrawBackground(IWidget widget, Action<Rect> drawBackground)
        : base(widget)
    {
        _DrawBackground = drawBackground;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        _DrawBackground(rect);

        Widget.Draw(rect, containerSize);
    }
}

public static partial class WidgetAPI
{
    public static DrawBackground Background(
        this IWidget widget,
        Action<Rect> drawBackground
    )
    {
        return new(widget, drawBackground);
    }
}
