using UnityEngine;

namespace Stats.Widgets.Extensions.Size;

public sealed class RelHeightWidgetExtension : WidgetExtension
{
    private readonly float ParentHeightMultiplier;
    internal RelHeightWidgetExtension(
        Widget widget,
        float parentHeightMultiplier
    ) : base(widget)
    {
        ParentHeightMultiplier = parentHeightMultiplier;
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        {
            y = containerSize.y * ParentHeightMultiplier
        };
    }
}
