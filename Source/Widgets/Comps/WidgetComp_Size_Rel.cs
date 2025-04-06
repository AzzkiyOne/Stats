using UnityEngine;

namespace Stats;

public class WidgetComp_Size_Rel
    : WidgetComp
{
    private readonly float WidthMult;
    private readonly float HeightMult;
    public WidgetComp_Size_Rel(IWidget widget, float widthMult, float heightMult)
        : base(widget)
    {
        WidthMult = widthMult;
        HeightMult = heightMult;
        widget.WidthIsUndef = false;
        widget.HeightIsUndef = false;
    }
    public WidgetComp_Size_Rel(IWidget widget, float sizeMult)
        : this(widget, sizeMult, sizeMult)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        Vector2 size = Widget.GetSize(containerSize);

        if (containerSize.x < float.PositiveInfinity)
            size.x = WidthMult * containerSize.x;
        if (containerSize.y < float.PositiveInfinity)
            size.y = HeightMult * containerSize.y;

        return size;
    }
}
