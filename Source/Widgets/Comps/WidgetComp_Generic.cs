using UnityEngine;

namespace Stats.Widgets.Comps;

public class WidgetComp_Generic
    : WidgetComp
{
    private readonly OnDrawCB OnDraw;
    public WidgetComp_Generic(ref IWidget widget, OnDrawCB onDraw)
        : base(ref widget)
    {
        OnDraw = onDraw;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        OnDraw(rect);

        Widget.Draw(rect, containerSize);
    }
    public delegate void OnDrawCB(Rect rect);
}
