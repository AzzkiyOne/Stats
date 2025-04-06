using UnityEngine;

namespace Stats;

public class WidgetComp_Width_Func
    : WidgetComp
{
    private readonly SizeFunc_SingleAxis WidthFunc;
    public WidgetComp_Width_Func(IWidget widget, SizeFunc_SingleAxis widthFunc)
        : base(widget)
    {
        WidthFunc = widthFunc;
        widget.WidthIsUndef = false;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);

        if (containerSize.x < float.PositiveInfinity)
            size.x = WidthFunc(containerSize);

        return size;
    }
}
