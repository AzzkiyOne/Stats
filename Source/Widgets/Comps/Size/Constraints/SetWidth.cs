using UnityEngine;

namespace Stats.Widgets.Comps.Size.Constraints;

public class SetWidth
    : WidgetComp
{
    private readonly SizeFunc_SingleAxis WidthFunc;
    public SetWidth(ref IWidget widget, SizeFunc_SingleAxis widthFunc)
        : base(ref widget)
    {
        WidthFunc = widthFunc;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = WidthFunc(containerSize) };
    }
}
