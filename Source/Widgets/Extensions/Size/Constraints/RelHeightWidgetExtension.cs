using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class RelHeightWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly float ParentHeightMultiplier;
    internal RelHeightWidgetExtension(
        Widget widget,
        float parentHeightMultiplier
    )
    {
        Widget = widget;
        ParentHeightMultiplier = parentHeightMultiplier;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        {
            y = containerSize.y * ParentHeightMultiplier
        };
    }
}
