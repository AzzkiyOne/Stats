using UnityEngine;

namespace Stats;

// Should only be applied to containers.
public class WidgetComp_Padding_Rel
    : WidgetComp
{
    private readonly float L;
    private readonly float R;
    private readonly float T;
    private readonly float B;
    public WidgetComp_Padding_Rel(IWidget widget, float l, float r, float t, float b)
        : base(widget)
    {
        L = l;
        R = r;
        T = t;
        B = b;
    }
    public WidgetComp_Padding_Rel(IWidget widget, float hor, float ver)
        : base(widget)
    {
        L = R = hor;
        T = B = ver;
    }
    public WidgetComp_Padding_Rel(IWidget widget, float amount)
        : base(widget)
    {
        L = R = T = B = amount;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);

        size.x *= (L + R);
        size.y *= (T + B);

        return size;
    }
    public override Vector2 GetSize()
    {
        Vector2 size = Widget.GetSize();

        size.x *= (L + R);
        size.y *= (T + B);

        return size;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        rect.x += L * rect.width;
        rect.y += T * rect.height;
        rect.width -= (L + R) * rect.width;
        rect.height -= (T + B) * rect.height;

        Widget.Draw(rect, containerSize);
    }
}
