using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class CalcWidthWidgetExtension
    : WidgetExtension
{
    private readonly SingleAxisSizeFunc WidthFunction;
    internal CalcWidthWidgetExtension(IWidget widget, SingleAxisSizeFunc widthFunction)
        : base(widget)
    {
        WidthFunction = widthFunction;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        { x = WidthFunction(containerSize) };
    }
}
