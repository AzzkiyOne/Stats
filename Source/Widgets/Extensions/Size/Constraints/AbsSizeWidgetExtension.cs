using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class AbsSizeWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly Vector2 Size;
    internal AbsSizeWidgetExtension(Widget widget, float width, float height)
    {
        Widget = widget;
        Size.x = width;
        Size.y = height;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Size;
    }
    public override Vector2 GetSize()
    {
        return Size;
    }
}
