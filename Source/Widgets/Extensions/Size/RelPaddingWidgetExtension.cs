using UnityEngine;

namespace Stats.Widgets.Extensions.Size;

public sealed class RelPaddingWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly float Left;
    private readonly float Top;
    private readonly float Horizontal;
    private readonly float Vertical;
    internal RelPaddingWidgetExtension(
        Widget widget,
        float left,
        float right,
        float top,
        float bottom
    )
    {
        Widget = widget;
        Left = left;
        Top = top;
        Horizontal = left + right;
        Vertical = top + bottom;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);
        size.x += Horizontal * containerSize.x;
        size.y += Vertical * containerSize.y;

        return size;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        GUIDebugger.DebugRect(this, rect);

        rect.x += Left * containerSize.x;
        rect.y += Top * containerSize.y;
        rect.width -= Horizontal * containerSize.x;
        rect.height -= Vertical * containerSize.y;

        Widget.Draw(rect, containerSize);
    }
}
