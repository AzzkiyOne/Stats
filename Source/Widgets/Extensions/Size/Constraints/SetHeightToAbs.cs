using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class SetHeightToAbs
    : WidgetExtension
{
    private readonly float Value;
    internal SetHeightToAbs(IWidget widget, float value)
        : base(widget)
    {
        Value = value;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { y = Value };
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize() with { y = Value };
    }
}

public static partial class WidgetAPI
{
    public static SetHeightToAbs HeightAbs(this IWidget widget, float value)
    {
        return new(widget, value);
    }
}
