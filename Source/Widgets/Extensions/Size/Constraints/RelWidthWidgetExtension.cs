using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class RelWidthWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly float ParentWidthMultiplier;
    internal RelWidthWidgetExtension(
        Widget widget,
        float parentWidthMultiplier
    )
    {
        Widget = widget;
        ParentWidthMultiplier = parentWidthMultiplier;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        {
            x = containerSize.x * ParentWidthMultiplier
        };
    }
}
