using UnityEngine;

namespace Stats.Widgets.Comps;

public class Draw
    : WidgetComp,
      IWidget
{
    private readonly OnDrawCB OnDraw;
    public Draw(ref IWidget widget, OnDrawCB onDraw)
        : base(ref widget)
    {
        OnDraw = onDraw;
    }
    void IWidget.Draw(Rect rect, in Vector2 containerSize)
    {
        OnDraw(rect);

        Widget.Draw(rect, containerSize);
    }
    public delegate void OnDrawCB(Rect rect);
}
