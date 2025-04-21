using UnityEngine;

namespace Stats.Widgets.Comps.Size.Constraints;

public class SetHeightToRel
    : WidgetComp
{
    private readonly float Mult;
    public SetHeightToRel(ref IWidget widget, float mult)
        : base(ref widget)
    {
        Mult = mult;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { y = containerSize.y * Mult };
    }
}
