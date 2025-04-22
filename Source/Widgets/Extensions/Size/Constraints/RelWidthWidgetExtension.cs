using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class RelWidthWidgetExtension
    : WidgetExtension
{
    private readonly float ParentWidthMultiplier;
    internal RelWidthWidgetExtension(
        IWidget widget,
        float parentWidthMultiplier
    ) : base(widget)
    {
        ParentWidthMultiplier = parentWidthMultiplier;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        {
            x = containerSize.x * ParentWidthMultiplier
        };
    }
}
