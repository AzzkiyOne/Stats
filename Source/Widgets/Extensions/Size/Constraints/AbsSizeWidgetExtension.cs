using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class AbsSizeWidgetExtension
    : WidgetExtension
{
    private readonly Vector2 Size;
    internal AbsSizeWidgetExtension(IWidget widget, float width, float height)
        : base(widget)
    {
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
