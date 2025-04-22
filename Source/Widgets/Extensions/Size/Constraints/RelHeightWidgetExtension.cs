using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class RelHeightWidgetExtension
    : WidgetExtension
{
    private readonly float ParentHeightMultiplier;
    internal RelHeightWidgetExtension(
        IWidget widget,
        float parentHeightMultiplier
    ) : base(widget)
    {
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
