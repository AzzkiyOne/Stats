using UnityEngine;

namespace Stats.Widgets.Extensions.Size.Constraints;

public sealed class CalcWidthWidgetExtension
    : WidgetDecorator
{
    public override Widget Widget { get; }
    private readonly SingleAxisSizeFunc WidthFunction;
    internal CalcWidthWidgetExtension(
        Widget widget,
        SingleAxisSizeFunc widthFunction
    )
    {
        Widget = widget;
        WidthFunction = widthFunction;
    }
    public override Vector2 GetSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        { x = WidthFunction(containerSize) };
    }
}
