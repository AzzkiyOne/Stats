using UnityEngine;

namespace Stats.Widgets.Extensions.Size;

public sealed class AbsPaddingWidgetExtension
    : WidgetExtension
{
    private readonly float Left;
    private readonly float Top;
    private readonly float Horizontal;
    private readonly float Vertical;
    internal AbsPaddingWidgetExtension(
        IWidget widget,
        float left,
        float right,
        float top,
        float bottom
    ) : base(widget)
    {
        Left = left;
        Top = top;
        Horizontal = left + right;
        Vertical = top + bottom;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);
        size.x += Horizontal;
        size.y += Vertical;

        return size;
    }
    public override Vector2 GetSize()
    {
        Vector2 size = Widget.GetSize();
        size.x += Horizontal;
        size.y += Vertical;

        return size;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        GUIDebugger.DebugRect(this, rect);

        rect.x += Left;
        rect.y += Top;
        rect.width -= Horizontal;
        rect.height -= Vertical;

        Widget.Draw(rect, containerSize);
    }
}
