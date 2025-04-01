using UnityEngine;

namespace Stats;

public class WidgetComp_Height_Rel
    : WidgetComp
{
    private readonly float Mult;
    public WidgetComp_Height_Rel(IWidget widget, float mult)
        : base(widget)
    {
        Mult = mult;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);

        size.y = containerSize.y * Mult;

        return size;
    }
}
