using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class RelSizeWidgetExtension
    : WidgetExtension
{
    private readonly float ParentWidthMultiplier;
    private readonly float ParentHeightMultiplier;
    internal RelSizeWidgetExtension(
        IWidget widget,
        float parentWidthMultiplier,
        float parentHeightMultiplier
    ) : base(widget)
    {
        ParentWidthMultiplier = parentWidthMultiplier;
        ParentHeightMultiplier = parentHeightMultiplier;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size;
        size.x = ParentWidthMultiplier * containerSize.x;
        size.y = ParentHeightMultiplier * containerSize.y;

        return size;
    }
}
