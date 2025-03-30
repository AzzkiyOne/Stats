using UnityEngine;

namespace Stats;

public class Widget_Addon_Generic
    : Widget_Addon
{
    private readonly OnDrawCB OnDraw;
    public Widget_Addon_Generic(Widget widget, OnDrawCB onDraw)
        : base(widget)
    {
        OnDraw = onDraw;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        OnDraw(ref rect);

        base.Draw(rect, containerSize);
    }
    public delegate void OnDrawCB(ref Rect rect);
}
