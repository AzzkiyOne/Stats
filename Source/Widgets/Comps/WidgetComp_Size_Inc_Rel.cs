using UnityEngine;

namespace Stats;

public class WidgetComp_Size_Inc_Rel
    : WidgetComp
{
    private readonly float L;
    private readonly float R;
    private readonly float T;
    private readonly float B;
    public WidgetComp_Size_Inc_Rel(IWidget widget, float l, float r, float t, float b)
        : base(widget)
    {
        L = l;
        R = r;
        T = t;
        B = b;
    }
    public WidgetComp_Size_Inc_Rel(IWidget widget, float hor, float ver)
        : this(widget, hor, hor, ver, ver)
    {
    }
    public WidgetComp_Size_Inc_Rel(IWidget widget, float amount)
        : this(widget, amount, amount, amount, amount)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);

        size.x += (L + R) * containerSize.x;
        size.y += (T + B) * containerSize.y;

        return size;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        rect.x += L * containerSize.x;
        rect.y += T * containerSize.y;
        rect.width -= (L + R) * containerSize.x;
        rect.height -= (T + B) * containerSize.y;

        Widget.Draw(rect, containerSize);
    }
}
