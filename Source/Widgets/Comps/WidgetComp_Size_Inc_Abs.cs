using UnityEngine;

namespace Stats;

public class WidgetComp_Size_Inc_Abs
    : WidgetComp
{
    public override Vector2 AbsSize => Widget.AbsSize + new Vector2(L + R, T + B);
    private readonly float L;
    private readonly float R;
    private readonly float T;
    private readonly float B;
    public WidgetComp_Size_Inc_Abs(IWidget widget, float l, float r, float t, float b)
        : base(widget)
    {
        L = l;
        R = r;
        T = t;
        B = b;
    }
    public WidgetComp_Size_Inc_Abs(IWidget widget, float hor, float ver)
        : this(widget, hor, hor, ver, ver)
    {
    }
    public WidgetComp_Size_Inc_Abs(IWidget widget, float amount)
        : this(widget, amount, amount, amount, amount)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);

        size.x += L + R;
        size.y += T + B;

        return size;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        rect.x += L;
        rect.y += T;
        rect.width -= L + R;
        rect.height -= T + B;

        Widget.Draw(rect, containerSize);
    }
}
