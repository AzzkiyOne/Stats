using UnityEngine;

namespace Stats;

public class WidgetComp_Size_Rel
    : WidgetComp
{
    private readonly float WidthMult;
    private readonly float HeightMult;
    public WidgetComp_Size_Rel(IWidget widget, float widthMlut, float heightMult)
        : base(widget)
    {
        WidthMult = widthMlut;
        HeightMult = heightMult;
    }
    public WidgetComp_Size_Rel(IWidget widget, float sizeMult)
        : base(widget)
    {
        WidthMult = HeightMult = sizeMult;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size;

        size.x = WidthMult * containerSize.x;
        size.y = HeightMult * containerSize.y;

        return size;
    }
}
