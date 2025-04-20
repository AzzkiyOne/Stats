using UnityEngine;

namespace Stats.Widgets.Comps.Size.Constraints;

public class WidgetComp_Width_Abs
    : WidgetComp
{
    private readonly float Value;
    public WidgetComp_Width_Abs(ref IWidget widget, float value)
        : base(ref widget)
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
