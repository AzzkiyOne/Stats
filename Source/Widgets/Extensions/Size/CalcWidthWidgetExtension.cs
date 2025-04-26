using UnityEngine;

namespace Stats.Widgets.Extensions.Size;

public sealed class CalcWidthWidgetExtension : WidgetExtension
{
    private readonly SingleAxisSizeFunc WidthFunction;
    internal CalcWidthWidgetExtension(
        Widget widget,
        SingleAxisSizeFunc widthFunction
    ) : base(widget)
    {
        WidthFunction = widthFunction;

        Resize();
    }
    protected override Vector2 CalcSize(Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with
        { x = WidthFunction(containerSize) };
    }
}
