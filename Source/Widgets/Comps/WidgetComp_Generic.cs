using UnityEngine;

namespace Stats;

public class WidgetComp_Generic
    : WidgetComp
{
    private readonly OnDrawCB OnDraw;
    public WidgetComp_Generic(IWidget widget, OnDrawCB onDraw)
        : base(widget)
    {
        OnDraw = onDraw;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        OnDraw(ref rect);

        Widget.Draw(rect, containerSize);
    }
    public delegate void OnDrawCB(ref Rect rect);
}
