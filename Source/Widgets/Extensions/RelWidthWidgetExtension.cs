using UnityEngine;

namespace Stats.Widgets.Extensions;

public sealed class RelWidthWidgetExtension : WidgetExtension
{
    private readonly float ParentWidthMultiplier;
    internal RelWidthWidgetExtension(
        Widget widget,
        float parentWidthMultiplier
    ) : base(widget)
    {
        ParentWidthMultiplier = parentWidthMultiplier;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        {
            x = containerSize.x * ParentWidthMultiplier
        };
    }
}
