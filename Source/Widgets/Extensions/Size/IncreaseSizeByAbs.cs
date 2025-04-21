using UnityEngine;

namespace Stats.Widgets.Extensions.Size;

public sealed class IncreaseSizeByAbs
    : WidgetExtension
{
    private readonly float L;
    private readonly float T;
    private readonly float Hor;
    private readonly float Ver;
    internal IncreaseSizeByAbs(IWidget widget, float l, float r, float t, float b)
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

public static partial class WidgetAPI
{
    public static IncreaseSizeByAbs PadAbs(this IWidget widget, float l, float r, float t, float b)
    {
        return new(widget, l, r, t, b);
    }
    public static IncreaseSizeByAbs PadAbs(this IWidget widget, float hor, float ver)
    {
        return widget.PadAbs(hor, hor, ver, ver);
    }
    public static IncreaseSizeByAbs PadAbs(this IWidget widget, float amount)
    {
        return widget.PadAbs(amount, amount, amount, amount);
    }
}
