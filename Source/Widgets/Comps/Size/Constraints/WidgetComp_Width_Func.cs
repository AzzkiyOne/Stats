using UnityEngine;

namespace Stats;

public class WidgetComp_Width_Func
    : WidgetComp
{
    private readonly SizeFunc_SingleAxis WidthFunc;
    public WidgetComp_Width_Func(ref IWidget widget, SizeFunc_SingleAxis widthFunc)
        : base(ref widget)
    {
        WidthFunc = widthFunc;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = WidthFunc(containerSize) };
    }
}
