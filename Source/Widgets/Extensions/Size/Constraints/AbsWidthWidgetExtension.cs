using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class AbsWidthWidgetExtension
    : WidgetExtension
{
    private readonly float Width;
    internal AbsWidthWidgetExtension(IWidget widget, float width)
        : base(widget)
    {
        Width = width;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = Width };
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize() with { x = Width };
    }
}
