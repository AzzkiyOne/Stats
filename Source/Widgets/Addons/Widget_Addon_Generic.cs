using System;
using UnityEngine;

namespace Stats;

public class Widget_Addon_Generic
    : Widget_Addon
{
    private readonly Action<Rect> OnDraw;
    public Widget_Addon_Generic(Widget widget, Action<Rect> onDraw)
        : base(widget)
    {
        OnDraw = onDraw;
    }
    public override void Draw(Rect rect)
    {
        OnDraw(rect);

        base.Draw(rect);
    }
}
