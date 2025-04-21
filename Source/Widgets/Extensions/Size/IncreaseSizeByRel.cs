using UnityEngine;

namespace Stats.Widgets.Extensions.Size;

public sealed class IncreaseSizeByRel
    : WidgetExtension
{
    private readonly float L;
    private readonly float T;
    private readonly float Hor;
    private readonly float Ver;
    internal IncreaseSizeByRel(IWidget widget, float l, float r, float t, float b)
        : base(widget)
    {
        L = l;
        T = t;
        Hor = l + r;
        Ver = t + b;
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

public static partial class WidgetAPI
{
    public static IncreaseSizeByRel PadRel(this IWidget widget, float l, float r, float t, float b)
    {
        return new(widget, l, r, t, b);
    }
    public static IncreaseSizeByRel PadRel(this IWidget widget, float hor, float ver)
    {
        return widget.PadRel(hor, hor, ver, ver);
    }
    public static IncreaseSizeByRel PadRel(this IWidget widget, float amount)
    {
        return widget.PadRel(amount, amount, amount, amount);
    }
}
