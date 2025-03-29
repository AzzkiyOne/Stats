using UnityEngine;

namespace Stats;

public abstract class Widget_Addon_RectOffset
    : Widget_Addon
{
    protected float Left { get; }
    protected float Top { get; }
    protected float Hor { get; }
    protected float Ver { get; }
    public Widget_Addon_RectOffset(Widget widget, float l, float r, float t, float b)
        : base(widget)
    {
        Left = l;
        Top = t;
        Hor = l + r;
        Ver = t + b;
    }
    public Widget_Addon_RectOffset(Widget widget, (float hor, float ver) vals)
        : this(widget, vals.hor, vals.hor, vals.ver, vals.ver)
    {
    }
    public override void Draw(Rect rect)
    {
        rect.x += Left;
        rect.y += Top;
        rect.width -= Hor;
        rect.height -= Ver;

        Widget.Draw(rect);
    }
}
