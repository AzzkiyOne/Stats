using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class SetWidth
    : WidgetExtension
{
    private readonly SingleAxisSizeFunc WidthFunc;
    internal SetWidth(IWidget widget, SingleAxisSizeFunc widthFunc)
        : base(widget)
    {
        WidthFunc = widthFunc;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = WidthFunc(containerSize) };
    }
}

public static partial class WidgetAPI
{
    public static SetWidth Width(this IWidget widget, SingleAxisSizeFunc widthFunc)
    {
        return new(widget, widthFunc);
    }
}
