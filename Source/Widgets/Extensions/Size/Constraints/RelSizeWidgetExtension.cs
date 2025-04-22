using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class RelSizeWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly float ParentWidthMultiplier;
    private readonly float ParentHeightMultiplier;
    internal RelSizeWidgetExtension(
        Widget widget,
        float parentWidthMultiplier,
        float parentHeightMultiplier
    )
    {
        Widget = widget;
        ParentWidthMultiplier = parentWidthMultiplier;
        ParentHeightMultiplier = parentHeightMultiplier;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        Vector2 size;
        size.x = ParentWidthMultiplier * containerSize.x;
        size.y = ParentHeightMultiplier * containerSize.y;

        return size;
    }
}
