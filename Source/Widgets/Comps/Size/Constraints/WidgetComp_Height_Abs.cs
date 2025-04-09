using UnityEngine;

namespace Stats;

public class WidgetComp_Height_Abs
    : WidgetComp
{
    private readonly float Value;
    public WidgetComp_Height_Abs(ref IWidget widget, float value)
        : base(ref widget)
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
