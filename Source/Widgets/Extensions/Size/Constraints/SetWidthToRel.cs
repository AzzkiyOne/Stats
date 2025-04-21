using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class SetWidthToRel
    : WidgetExtension
{
    private readonly float Mult;
    internal SetWidthToRel(IWidget widget, float mult)
        : base(widget)
    {
        Mult = mult;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = containerSize.x * Mult };
    }
}

public static partial class WidgetAPI
{
    public static SetWidthToRel WidthRel(this IWidget widget, float mult)
    {
        return new(widget, mult);
    }
}
