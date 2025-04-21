using UnityEngine;

namespace Stats.Widgets.Comps.Size;

public class IncreaseSizeByRel
    : WidgetComp
{
    private readonly float L;
    private readonly float T;
    private readonly float Hor;
    private readonly float Ver;
    public IncreaseSizeByRel(ref IWidget widget, float l, float r, float t, float b)
        : base(ref widget)
    {
        L = l;
        T = t;
        Hor = l + r;
        Ver = t + b;
    }
    public IncreaseSizeByRel(ref IWidget widget, float hor, float ver)
        : this(ref widget, hor, hor, ver, ver)
    {
    }
    public IncreaseSizeByRel(ref IWidget widget, float amount)
        : this(ref widget, amount, amount, amount, amount)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);
        size.x += Hor * containerSize.x;
        size.y += Ver * containerSize.y;

        return size;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        GUIDebugger.DebugRect(this, rect);

        rect.x += L * containerSize.x;
        rect.y += T * containerSize.y;
        rect.width -= Hor * containerSize.x;
        rect.height -= Ver * containerSize.y;

        Widget.Draw(rect, containerSize);
    }
}
