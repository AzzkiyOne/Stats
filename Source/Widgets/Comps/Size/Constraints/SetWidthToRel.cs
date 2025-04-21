using UnityEngine;

namespace Stats.Widgets.Comps.Size.Constraints;

public class SetWidthToRel
    : WidgetComp
{
    private readonly float Mult;
    public SetWidthToRel(ref IWidget widget, float mult)
        : base(ref widget)
    {
        Mult = mult;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { x = containerSize.x * Mult };
    }
}
