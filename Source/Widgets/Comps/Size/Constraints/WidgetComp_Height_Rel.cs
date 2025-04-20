using UnityEngine;

namespace Stats.Widgets.Comps.Size.Constraints;

public class WidgetComp_Height_Rel
    : WidgetComp
{
    private readonly float Mult;
    public WidgetComp_Height_Rel(ref IWidget widget, float mult)
        : base(ref widget)
    {
        Mult = mult;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize) with { y = containerSize.y * Mult };
    }
}
