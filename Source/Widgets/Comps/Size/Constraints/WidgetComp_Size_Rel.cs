using UnityEngine;

namespace Stats.Widgets.Comps.Size.Constraints;

public class WidgetComp_Size_Rel
    : WidgetComp
{
    private readonly float WidthMult;
    private readonly float HeightMult;
    public WidgetComp_Size_Rel(ref IWidget widget, float widthMult, float heightMult)
        : base(ref widget)
    {
        WidthMult = widthMult;
        HeightMult = heightMult;
    }
    public WidgetComp_Size_Rel(ref IWidget widget, float sizeMult)
        : this(ref widget, sizeMult, sizeMult)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size;
        size.x = WidthMult * containerSize.x;
        size.y = HeightMult * containerSize.y;

        return size;
    }
}
