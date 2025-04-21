using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class SetHeightToRel
    : WidgetExtension
{
    private readonly float Mult;
    internal SetHeightToRel(IWidget widget, float mult)
        : base(widget)
    {
        Mult = mult;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { y = containerSize.y * Mult };
    }
}

public static partial class WidgetAPI
{
    public static SetHeightToRel HeightRel(this IWidget widget, float value)
    {
        return new(widget, value);
    }
}
