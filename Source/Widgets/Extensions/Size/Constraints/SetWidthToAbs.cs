using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class SetWidthToAbs
    : WidgetExtension
{
    private readonly float Value;
    internal SetWidthToAbs(IWidget widget, float value)
        : base(widget)
    {
        Value = value;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = Value };
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize() with { x = Value };
    }
}

public static partial class WidgetAPI
{
    public static SetWidthToAbs WidthAbs(this IWidget widget, float value)
    {
        return new(widget, value);
    }
}
