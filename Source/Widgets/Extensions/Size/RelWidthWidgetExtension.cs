using UnityEngine;

namespace Stats.Widgets.Extensions.Size;

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
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        {
            x = containerSize.x * ParentWidthMultiplier
        };
    }
}
