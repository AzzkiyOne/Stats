using UnityEngine;

namespace Stats;

public class WidgetComp_Size_Inc_Abs
    : WidgetComp
{
    private readonly float L;
    private readonly float T;
    private readonly float Hor;
    private readonly float Ver;
    public WidgetComp_Size_Inc_Abs(ref IWidget widget, float l, float r, float t, float b)
        : base(ref widget)
    {
        L = l;
        T = t;
        Hor = l + r;
        Ver = t + b;
    }
    public WidgetComp_Size_Inc_Abs(ref IWidget widget, float hor, float ver)
        : this(ref widget, hor, hor, ver, ver)
    {
    }
    public WidgetComp_Size_Inc_Abs(ref IWidget widget, float amount)
        : this(ref widget, amount, amount, amount, amount)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);
        size.x += Hor;
        size.y += Ver;

        return size;
    }
    public override Vector2 GetSize()
    {
        Vector2 size = Widget.GetSize();
        size.x += Hor;
        size.y += Ver;

        return size;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        GUIDebugger.DebugRect(this, rect);

        rect.x += L;
        rect.y += T;
        rect.width -= Hor;
        rect.height -= Ver;

        Widget.Draw(rect, containerSize);
    }
}
