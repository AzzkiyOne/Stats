using UnityEngine;

namespace Stats.Widgets.Comps.Size.Constraints;

public class SetWidthToAbs
    : WidgetComp
{
    private readonly float Value;
    public SetWidthToAbs(ref IWidget widget, float value)
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
